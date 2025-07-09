using UnityEngine;
using System.Collections;
using System.Numerics;
using System.Threading;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;

    
    [SerializeField] int HP;

   
    [SerializeField] Gun pistolDrop, SMGDrop, RifleDrop, SniperDrop;
    [SerializeField] int dropChance;
    Color colorOrg;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrg = model.material.color;
        //Notify Manager that this enemy is in the level
        gameManager.instance.enemyCount += 1;
    }

    // Update is called once per frame
    void Update()
    {
        //Hello 0/
    }

    //Needed so you can implement Damage
    public void takeDamage(int amount)
    {
        HP -= amount;


        if (HP <= 0)
        {
            //Keeping track of the enemy count 
            //gameManager.instance.enemyCount -= 1;
            if (Random.Range(0, 100) <= dropChance)
            {
                DropRandomGun(); ;
            }
            Destroy(gameObject);
        }
        else
        {
            //Only will flash if they're still alive
            StartCoroutine(flashRed());
        }
    }

    //FeedBack to player 
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrg;
    }

    void DropRandomGun()
    {
        int num = Random.Range(0, 3);
        switch (num)
        {
            case 0:
                {
                    Gun gun = Instantiate (pistolDrop, transform.position, transform.rotation);
                    gun.mtype = Gun.WeaponClass.Pistol;
                    gun.mDMG = Random.Range(1, 10);
                    gun.mFireRate = Random.Range(.25f, 2);
                    gun.mMaxAmmo = Random.Range(45, 90);
                    gun.mMaxMag = Random.Range(5, 15);
                    gun.mRange = Random.Range(50, 150);
                    gun.mReloadSpeed = Random.Range(3, 6);
                    
                    break;
                }
            case 1:
                {
                    Gun gun = Instantiate(SMGDrop, transform.position, transform.rotation);
                    gun.mtype = Gun.WeaponClass.SMG;
                    gun.mDMG = Random.Range(3, 10);
                    gun.mFireRate = Random.Range(.05f, .5f);
                    gun.mMaxAmmo = Random.Range(300, 750);
                    gun.mMaxMag = Random.Range(30, 60);
                    gun.mRange = Random.Range(40, 120);
                    gun.mReloadSpeed = Random.Range(2, 5);
                    
                    break;
                }
            case 2:
                {
                    Gun gun = Instantiate(RifleDrop, transform.position, transform.rotation);
                    gun.mtype = Gun.WeaponClass.Rifle;
                    gun.mDMG = Random.Range(10, 20);
                    gun.mFireRate = Random.Range(.2f, 2);
                    gun.mMaxAmmo = Random.Range(300, 450);
                    gun.mMaxMag = Random.Range(15, 40);
                    gun.mRange = Random.Range(150, 250);
                    gun.mReloadSpeed = Random.Range(4, 8);
                    
                    break;
                }
            default:
                {
                    Gun gun = Instantiate(SniperDrop, transform.position, transform.rotation);
                    gun.mtype = Gun.WeaponClass.Sniper;
                    gun.mDMG = Random.Range(50, 100);
                    gun.mFireRate = Random.Range(2f, 5);
                    gun.mMaxAmmo = Random.Range(30, 60);
                    gun.mMaxMag = Random.Range(5, 10);
                    gun.mRange = Random.Range(200, 300);
                    gun.mReloadSpeed = Random.Range(5, 9);
                    
                    break;
                }

        }

    }
}
