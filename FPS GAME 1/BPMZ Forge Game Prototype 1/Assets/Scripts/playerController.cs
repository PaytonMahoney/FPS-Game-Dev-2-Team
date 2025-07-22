using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class playerController : MonoBehaviour, IDamage, IHeal, IPickUp
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
    public int maxHP;
    
    // Movement
    [Range(5, 40)] [SerializeField] public int moveSpeed;
    [Range(1, 5)] [SerializeField] int sprintMod;
    [SerializeField] public Transform centerMass;
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
    [SerializeField] float gravity;
    float gravityOrig;

    //Shooting
    //[SerializeField] public Gun equipGun;
    [SerializeField] GameObject gunModel;
    [SerializeField] public Transform shootPOS;
    [SerializeField] public List<Gun> gunInventory = new List<Gun>();
    public Gun currentGun;
    private int gunListPosition;
    private bool isReloading;

    //Items
    public Item activeItem;
    [SerializeField] public List<Item> itemInventory = new List<Item>();

    //Item Modifiers
    public int DMGreduction;

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
        gravityOrig = gravity;
        DMGreduction = 0;
        


       // currentGun.ammoCurrent = currentGun.ammoMax;
       // currentGun.magCurrent = currentGun.magMax;
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

        if (activeItem != null && !activeItem.inUse && activeItem.currentCooldown < activeItem.itemCooldown)
        {
            activeItem.currentCooldown += Time.deltaTime;
        }
        UpdateFootstepDelay();
        
        footstepTimer += Time.deltaTime;

        //Tie A and D keys to the player character   //Strafe forward
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        //Time.deltaTime = Ignores Frame Rate
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        

        if (controller.isGrounded)
        {
            
            jumpCount = 0;
            playerVel = Vector3.zero;           
            
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
        //Crouching or nah
        if (state != MovementState.crouching)
        {
            jump();
            dash();
        }

        //footstepTimer = 0f; //In the air
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        // Fire away
        if (Input.GetButton("Fire1") && gunInventory.Count > 0 && shootTimer > currentGun.shootRate && !isReloading)
        {
            shoot();
            shootTimer = 0;
        }

        if (Input.GetButtonDown("Reload") && !isReloading)
        {
            reload();
            
        }

        if (activeItem != null && Input.GetButtonDown("Active Item") && activeItem.currentCooldown >= activeItem.itemCooldown)
        {
            activeItem.Activate();
            activeItem.currentCooldown = 0;
            if (activeItem.itemDuration > 0)
            {
                StartCoroutine(ItemWait(activeItem.itemDuration));
            }
        }
        selectGun();
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
            moveSpeed /= sprintMod;
        }
    }

    void shoot()
    {
        currentGun.shoot(ignoreLayer, playerAudio);
       
        gameManager.instance.updateAmmoPanel();
    }


    public void takeDamage(int amount)
    {
        amount = amount * (100 - DMGreduction) / 100;
        if (amount <= 0)
        {
            amount = 1;
        }
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
        gameManager.instance.playerHPText.text = HP.ToString() + " / " + maxHP.ToString();
       
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
    void reload()
    {
        if (currentGun.magCurrent == currentGun.magMax || currentGun.ammoCurrent == 0)
        {
            return;
        }
        isReloading = true;
        StartCoroutine(currentGun.reload(playerAudio));

        
        StartCoroutine(Wait(currentGun.reloadTime));
        
    }

    void UseActiveItem()
    {

    }

    public void PickUpItem(Item item)
    {
        if (item.type == Item.itemType.Passive)
        {
            itemInventory.Add(item);
            item.OnPickup();
        }
        else
        {
            activeItem = item;
            item.currentCooldown = item.itemCooldown;
            gameManager.instance.activeItemImage.sprite = item.icon;
        }
    }

    public void PickUpGun(Gun gun)
    {
        gunInventory.Add(gun);
        gunListPosition = gunInventory.Count - 1;
        if (gun.projectile != null)
        {
            gun.shootPOS = shootPOS;
        }
        changeGun();
        
    }

    void changeGun()
    {
        currentGun = gunInventory[gunListPosition];
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunInventory[gunListPosition].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunInventory[gunListPosition].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        gameManager.instance.updateAmmoPanel();
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPosition < gunInventory.Count - 1)
        {
            gunListPosition++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPosition > 0)
        {
            gunListPosition--;
            changeGun();
        }
    }
    IEnumerator Wait(float sec)
    {
        yield return  new WaitForSeconds(sec);
        isReloading = false;
    }

    IEnumerator ItemWait(float sec)
    {
        yield return new WaitForSeconds(sec);
        activeItem.Deactivate();
    }
}
