using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class playerController : MonoBehaviour, IDamage, IHeal
{
    //audio fields
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private AudioClip[] hurtClips;
    [SerializeField] private AudioClip[] footstepClips;
    [Range(0.25f, 1f)] [SerializeField] private float footstepDelay;
    private float originalFootstepDelay;
    private float footstepTimer;
    
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    
    [SerializeField] int HP;
    int maxHP;
    
    // Movement
    [Range(5, 20)] [SerializeField] int moveSpeed;
    [Range(1, 5)] [SerializeField] int sprintMod;
    private int moveSpeedOrig;

    private MovementState state;
    private MovementState stateOrig;
    public enum MovementState
    {
        walking,
        air,
        sprinting,
        crouching
    }
    
    //Crouching
    [SerializeField] int crouchHeight;
    private float standingHeight;
    
    //Jumping 
    [SerializeField] int jumpVel;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    //Shooting
    //[SerializeField] public Gun equipGun;
    [SerializeField] public List<GunStats> gunInventory = new List<GunStats>();
    public GunStats currentGun;
    private int gunListPosition;
    private bool isReloading;
    
    //Slope Handling
    [SerializeField] float maxSlopeAngle;
    [SerializeField] float slopeForce;
    private RaycastHit slopeHit;

    //Dash Handling
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    
    Vector3 moveDir;
    Vector3 playerVel;
    Vector3 dashVelocity;
    
    int jumpCount;
    float shootTimer;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        stateOrig = state;
        standingHeight = transform.localScale.y;
        moveSpeedOrig = moveSpeed;
        maxHP = HP;
        originalFootstepDelay = footstepDelay;
        updatePlayerUI();
    }

    // Update is called once per frame    //Should be on input functions
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.K))
        {
            takeDamage(10);
        }
        */
        //Drawing it so I can see it in action
        if(gunInventory.Count > 0)
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * currentGun.shootDistance, Color.violet);
        
        if(!gameManager.instance.isPaused)
            movement();
        
        crouch();
        sprint();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;
        
        //Tie A and D keys to the player character   //Strafe forward
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        //Time.deltaTime = Ignores Frame Rate
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        UpdateFootstepDelay();
        
        footstepTimer += Time.deltaTime;
        if (controller.isGrounded)
        {
            //Now Gravity won't stack
            Debug.Log("Grounded");
            playerVel = Vector3.zero;
            jumpCount = 0;
            if (OnSlope())
            {
                Vector3 slopeMove = GetSlopeMoveDirection() * moveSpeed;
                slopeMove += Vector3.down * slopeForce;
                controller.Move(slopeMove * Time.deltaTime);

            }
            else
            {
                controller.Move(playerVel * Time.deltaTime);
            }
            
            if (moveDir.x != 0 || moveDir.z != 0)
            {
                //Debug.Log("Moving");
                //footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepDelay)
                {
                    PlayFootstep();
                    footstepTimer = 0f;
                }
            }
            else
            {
                footstepTimer = 0f;
            }
        }
        else
        {
            //footstepTimer = 0f;
            controller.Move(playerVel * Time.deltaTime);
            playerVel.y -= gravity * Time.deltaTime;
            //jumpCount = 0;
        }

        // Crouching or nah
        if (state != MovementState.crouching)
        {
            jump();
            dash();
        }
        
        // Fire away
        if (Input.GetButton("Fire1") && gunInventory.Count > 0 && shootTimer > currentGun.shootRate && !isReloading)
        {
            shoot();
        }

        if (Input.GetButtonDown("Reload") && !isReloading)
        {
            reload();
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length > 0)
        {
            playerAudio.PlayOneShot(footstepClips[UnityEngine.Random.Range(0, footstepClips.Length)]);
        }
    }

    void UpdateFootstepDelay()
    {
        if (state == MovementState.sprinting)
            footstepDelay = originalFootstepDelay / 2;
        else if (state == MovementState.walking)
            footstepDelay = originalFootstepDelay;
        else if (state == MovementState.crouching)
            footstepDelay = originalFootstepDelay * 2;
        else
            footstepDelay = originalFootstepDelay; // Default
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpVel;
            jumpCount++;
        }
    }

    void crouch()
    {
        if(Input.GetButtonDown("Crouch"))
        {
            state = MovementState.crouching;
            moveSpeed = moveSpeedOrig / 2;
            transform.localScale = new Vector3(transform.localScale.x, standingHeight / 2, transform.localScale.z);
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            state = stateOrig;
            transform.localScale = new Vector3(transform.localScale.x, standingHeight, transform.localScale.z);
            moveSpeed = moveSpeedOrig;
        }
    }

    void dash()
    {
        if(Input.GetButtonDown("Dash"))
        {
            StartCoroutine(Dash());
        }
        
    }
    IEnumerator Dash()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            controller.Move(moveDir * dashSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            state = MovementState.sprinting;
            moveSpeed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            state = stateOrig;
            moveSpeed = moveSpeedOrig;
        }
    }

    void shoot()
    {
        if (currentGun.ammoCurrent > 0)
        {
            currentGun.ammoCurrent--;
            shootTimer = 0;
            RaycastHit hit;

            //First person view location
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit,
                    currentGun.shootDistance,
                    ~ignoreLayer))
            {
                playerAudio.PlayOneShot(currentGun.shootingSound);
                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    //Debug.Log(equipGun.mDMG);
                    dmg.takeDamage(currentGun.shootDMG);
                }
            }
        }
        else
        {
            playerAudio.PlayOneShot(currentGun.emptyShotSound);
        }
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, standingHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0; 
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }


    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(DamageFlashScreen());
        if (HP <= 0)
        {
            //YOU DIED SCREEN HERE
            gameManager.instance.youLose();
        }

        if (hurtClips.Length > 0)
        {
            int rand = UnityEngine.Random.Range(0, hurtClips.Length);
            playerAudio.PlayOneShot(hurtClips[rand]);
        }
    }

    public bool Heal(int amount)
    {
        if (HP < maxHP)   
        {
            StartCoroutine(HealFlashScreen());
            HP += amount;
            
            if (HP > maxHP)
            {
                HP = maxHP;
            }
            updatePlayerUI();
            return true;
        }
        return false;
    }

    public void updatePlayerUI()
    {
        //Debug.Log("HP UI Updated: " + HP + "/" + maxHP);
        gameManager.instance.playerHPBar.fillAmount = (float)HP / maxHP;
    }

    IEnumerator DamageFlashScreen()
    {
        gameManager.instance.playerDMGPanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDMGPanel.SetActive(false);
    }
    IEnumerator HealFlashScreen()
    {
        gameManager.instance.playerHealPanel.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        gameManager.instance.playerHealPanel.SetActive(false);
    }
    IEnumerator reload()
    {
        isReloading = true;
        playerAudio.PlayOneShot(currentGun.reloadSound);
        yield return new WaitForSeconds(currentGun.reloadTime);
        isReloading = false;
        currentGun.ammoCurrent = currentGun.ammoMax;
    }
}
