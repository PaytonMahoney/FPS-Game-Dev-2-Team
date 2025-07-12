using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class playerController : MonoBehaviour, IDamage, IHeal
{

    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    
    [SerializeField] int HP;
    int maxHP;
    
    // Movement
    [SerializeField] int moveSpeed;
    [SerializeField] int sprintMod;
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

    private GameObject gunUIActive;

    [SerializeField] private TMP_Text BulletCountUIText;
    [SerializeField] public GameObject PistolUI;
    [SerializeField] public GameObject RifleUI;
    [SerializeField] public GameObject SMGUI;
    [SerializeField] public GameObject SniperUI;
    
    //Crouching
    [SerializeField] int crouchHeight;
    private float standingHeight;
    
    //Jumping 
    [SerializeField] int jumpVel;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    //Shooting
    [SerializeField] public Gun equipGun;
    
    [SerializeField] AudioClip shootingSoundClip;
    [SerializeField] AudioClip emptyShotSoundClip;
    [SerializeField] AudioClip reloadSoundClip;
    [SerializeField] AudioSource audioSource;
    
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
        stateOrig = state;
        standingHeight = transform.localScale.y;
        moveSpeedOrig = moveSpeed;
        maxHP = HP;
        gunUIActive = null;
        equipGun.currentAmmo = equipGun.mMaxAmmo;
        equipGun.currentMag = equipGun.mMaxMag;
    }

    // Update is called once per frame    //Should be on input functions
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            takeDamage(10);
        }
        //Drawing it so I can see it in action
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * equipGun.mRange, Color.violet);
        movement();
        UpdateGunUI();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;
        

        //Tie A and D keys to the player character   //Strafe forward
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        //Time.deltaTime = Ignores Frame Rate
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        if (OnSlope())
        {
            Vector3 slopeMove = GetSlopeMoveDirection() * moveSpeed;

            slopeMove += Vector3.down * slopeForce;

            controller.Move(slopeMove * Time.deltaTime);
        }
        else if (controller.isGrounded)
        {
            //Now Gravity won't stack
            playerVel = Vector3.zero;
            jumpCount = 0;           
            controller.Move(playerVel * Time.deltaTime);
        }
        else
        {
            controller.Move(playerVel * Time.deltaTime);
            playerVel.y -= gravity * Time.deltaTime;
            //jumpCount = 0;
        }

       
        crouch();

        
        if (state != MovementState.crouching)
        {
            jump();
            sprint();
            dash();
        }
        
        if (Input.GetButton("Fire1") && shootTimer > equipGun.mFireRate)
        {
            shoot();
        }
        if (Input.GetButtonDown("Reload"))
        {
            
            StartCoroutine(ReloadGun());
        }
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
        if (equipGun.currentMag > 0)
        {
            shootTimer = 0;
            RaycastHit hit;
            //First person view location
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, equipGun.mRange,
                    ~ignoreLayer))
            {
                //Debug.Log(equipGun.currentAmmo);
                equipGun.currentMag--;
                audioSource.PlayOneShot(shootingSoundClip);
                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    //Debug.Log(equipGun.mDMG);
                    dmg.takeDamage(equipGun.mDMG);
                }
            }
            else
            {
                equipGun.currentMag--;
                audioSource.PlayOneShot(shootingSoundClip);
            }
        }
        else
        {
            shootTimer = 0;
            /*
            if (equipGun.currentAmmo > 0 )
            {
                equipGun.ReloadGun();
            }
            */
            audioSource.PlayOneShot(emptyShotSoundClip);
        }
        
    }
    
    IEnumerator ReloadGun()
    {
       // if (Input.GetKeyDown)
       audioSource.PlayOneShot(reloadSoundClip);
       yield return new WaitForSeconds(equipGun.mReloadSpeed);
       equipGun.ReloadGun();
       
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

    public GameObject GetGunUIType()
    {
        switch (equipGun.mtype)
        {
            case Gun.WeaponClass.Pistol:
                return PistolUI;
            case Gun.WeaponClass.SMG:
                return SMGUI;
            case Gun.WeaponClass.Rifle:
                return RifleUI;
            case Gun.WeaponClass.Sniper:
                return SniperUI;
            default:
                return PistolUI;
        }
    }
    
    public void UpdateGunUI()
    {
        //Debug.Log("Step 1");
        if (gunUIActive == null)
        {
            gunUIActive = GetGunUIType();
            gunUIActive.SetActive(true);
            //Debug.Log("Step 1");
        }
        else
        {
            //Debug.Log("Step 2");
            gunUIActive.SetActive(false);
            gunUIActive = GetGunUIType();
            gunUIActive.SetActive(true);
        }

        BulletCountUIText.text = equipGun.currentMag.ToString() + " / " + equipGun.currentAmmo.ToString();

    }
}
