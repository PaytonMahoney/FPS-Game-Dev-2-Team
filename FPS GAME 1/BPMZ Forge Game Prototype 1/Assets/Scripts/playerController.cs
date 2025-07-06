using UnityEngine;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour, IDamage
{

    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;


    [SerializeField] int HP;
    
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
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

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
    }

    // Update is called once per frame    //Should be on input functions
    void Update()
    {
        //Drawing it so I can see it in action
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.violet);
        
        movement();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        //Now Gravity won't stack
        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
        }


        //Tie A and D keys to the player character   //Strafe forward
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        //Time.deltaTime = Ignores Frame Rate
        controller.Move(moveDir * moveSpeed * Time.deltaTime);
        crouch();
        

        if (state != MovementState.crouching)
        {
            jump();
            sprint();
        }
        


        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer > shootRate)
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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            //Will tell me what the Raycast hit
            Debug.Log(hit.collider.name);

            //Damage code: Everything you need is here
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
    }



    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP < 0)
        {
            //YOU DIED SCREEN HERE
        }
    }
}
