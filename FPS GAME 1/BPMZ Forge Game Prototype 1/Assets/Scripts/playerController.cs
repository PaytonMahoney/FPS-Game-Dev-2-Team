using UnityEngine;
using System.Collections;

//using UnityEngine.InputSystem;

public class playerController : MonoBehaviour, IDamage, IHeal
{
    //audio fields
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip footstepSFX;
    [SerializeField] AudioClip gunShotSFX;
    [SerializeField] private AudioClip[] hurtClips;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepDelay = 0.4f;
    private float footstepTimer;




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


    //Crouching
    [SerializeField] int crouchHeight;
    private float standingHeight;
    

    //Jumping 
    [SerializeField] int jumpVel;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    //Shooting
    [SerializeField] public Gun equipGun;
    


    //Slope Handling
    [SerializeField] float maxSlopeAngle;
    [SerializeField] float slopeForce;
    private RaycastHit slopeHit;

    Vector3 moveDir;
    Vector3 playerVel;

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
    }

    void movement()
    {

        shootTimer += Time.deltaTime;

        //Tie A and D keys to the player character   //Strafe forward
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        //Time.deltaTime = Ignores Frame Rate
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        UpdateFootstepDelay();

        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= footstepDelay)
            {
                PlayFootstep();
                footstepTimer = 0f;
            }
        }
        else if (moveDir.magnitude < 0.01f || !controller.isGrounded)
        {
            footstepTimer = footstepDelay;
        }

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
        }

       

        if (Input.GetButton("Fire1") && shootTimer > equipGun.mFireRate && equipGun.currentMag > 0)
        {
            shoot();
        }

        if (controller.isGrounded && moveDir.magnitude > 0.1f && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(footstepSFX);
        }

    }

    void PlayFootstep()
    {
        if (footstepClips.Length > 0)
        {
            int rand = Random.Range(0, footstepClips.Length);
            playerAudio.PlayOneShot(footstepClips[rand]);
        }
    }

    void UpdateFootstepDelay()
    {
        if (state == MovementState.sprinting)
            footstepDelay = 0.3f;
        else if (state == MovementState.walking)
            footstepDelay = 0.5f;
        else if (state == MovementState.crouching)
            footstepDelay = 0.65f;
        else
            footstepDelay = 0.5f; // Default
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
        shootTimer = 0;

        RaycastHit hit;
        //First person view location
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, equipGun.mRange, ~ignoreLayer))
        {
            //Will tell me what the Raycast hit
            //Debug.Log(hit.collider.name);

            //Damage code: Everything you need is here
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(equipGun.mDMG);
            }
        }

        audioSource.PlayOneShot(gunShotSFX);
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
            int rand = Random.Range(0, hurtClips.Length);
            playerAudio.PlayOneShot(hurtClips[rand]);
        }
    }

    public bool Heal(int amount)
    {
        if (HP < maxHP)   
        {
            StartCoroutine(HealFlashScreen());
            HP += amount;
            updatePlayerUI();

            if (HP > maxHP)
            {
                HP = maxHP;
            }
            return true;
        }
        
            return false;
    }

    public void updatePlayerUI()
    {
        Debug.Log("HP UI Updated: " + HP + "/" + maxHP);
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
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerHealPanel.SetActive(false);
    }
}
