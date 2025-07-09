using UnityEngine;
using System.Collections;

//using UnityEngine.InputSystem;

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
        stateOrig = state;
        standingHeight = transform.localScale.y;
        moveSpeedOrig = moveSpeed;
        maxHP = HP;
        
    }

    // Update is called once per frame    //Should be on input functions
    void Update()
    {
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
            return true;
        }
        
            return false;
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
