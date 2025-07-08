using UnityEngine;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour, IDamage, IHeal
{

    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;


    [SerializeField] int HP;
    int HPMax;
    
    // Movement
    [SerializeField] int moveSpeed;
    [SerializeField] int sprintMod;
    private int moveSpeedOrig;

    private MovementState state; 
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
<<<<<<< Updated upstream
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
=======
    [SerializeField] Gun equip;
    
    // mDMG 
    // mFireRate 
    // mMaxAmmo = Maximum amount of ammo that can be held
    // mMaxMag = Max amount of ammo in the magazine
    // mRange
    // mReloadSpeed

    //Slope Handling
    [SerializeField] float maxSlopeAngle;
    [SerializeField] float slopeForce;
    private RaycastHit slopeHit;
>>>>>>> Stashed changes

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;

    float shootTimer;

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        standingHeight = transform.localScale.y;
        moveSpeedOrig = moveSpeed;
        HPMax = HP;
    }

    // Update is called once per frame    //Should be on input functions
    void Update()
    {
        //Drawing it so I can see it in action
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * equip.mRange, Color.violet);
        
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
        sprint();

        if (state != MovementState.crouching)
        {
            jump();
        }
        


        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer > equip.mFireRate && equip.currentMag > 0)
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
            moveSpeed = moveSpeed / 2;
            transform.localScale = new Vector3(transform.localScale.x, standingHeight / 2, transform.localScale.z);

        }
        else if (Input.GetButtonUp("Crouch"))
        {
            state = MovementState.walking;
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
            state = MovementState.walking;
            moveSpeed /= sprintMod;
        }
       
    }

    void shoot()
    {
        shootTimer = 0;
        equip.currentMag--;
        RaycastHit hit;
        //First person view location
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, equip.mRange, ~ignoreLayer))
        {
            //Will tell me what the Raycast hit
            Debug.Log(hit.collider.name);

            //Damage code: Everything you need is here
           
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(equip.mDMG);
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

    public bool Heal(int amount)
    {
        bool isHealed = false;
        if (HP < HPMax)
        {
            HP += amount;
            isHealed = true;
        }
        if (HP > HPMax) { 
        HP = HPMax;
        }
        return isHealed;
    }

    public void equipGun(Gun newgun)
    {
        //Instantiate(equip, newgun.transform);
        equip = newgun;
        
    }
}
