using UnityEngine;
using UnityEngine.UIElements;

public class Structures : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    [SerializeField] int speed;
    [SerializeField] int startDelay;
   
    
    [SerializeField] int rotateSpeed;

    [SerializeField] bool dropLoot;
   // [SerializeField] Gun pistolDrop, SMGDrop, RifleDrop, SniperDrop;
    float delayTimer;
    bool forward;
    Vector3 endposition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (end != null)
        {
            endposition = end.position;
            forward = true;
            delayTimer = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (end != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, endposition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, endposition) < 0.01f)
            {
                delayTimer += Time.deltaTime;
                if (delayTimer > startDelay)
                {
                    if (forward)
                    {
                        endposition = start.position;
                        forward = false;
                    }
                    else
                    {
                        endposition = end.position;
                        forward = true;
                    }
                    delayTimer = 0;
                }
            }
        }
        if (rotateSpeed > 0)
        {
            transform.Rotate(0, rotateSpeed, 0);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && dropLoot)
        {
            GunManager.instance.DropRandomGun(transform);
            Destroy(gameObject);
        }
    }
    //void DropRandomGun()
    //{
    //    int num = Random.Range(0, 4);
    //    Vector3 position = transform.position;
    //    position.y += 0.5f;
    //    switch (num)
    //    {
    //        case 0:
    //            {
    //                Gun gun = Instantiate(pistolDrop, position, transform.rotation);
    //                gun.mtype = Gun.WeaponClass.Pistol;
    //                gun.mDMG = Random.Range(7, 15);
    //                gun.mFireRate = Random.Range(.25f, .5f);
    //                gun.mMaxAmmo = Random.Range(45, 90);
    //                gun.mMaxMag = Random.Range(5, 15);
    //                gun.mRange = Random.Range(50, 150);
    //                gun.mReloadSpeed = Random.Range(1, 2);

    //                break;
    //            }
    //        case 1:
    //            {
    //                Gun gun = Instantiate(SMGDrop, position, transform.rotation);
    //                gun.mtype = Gun.WeaponClass.SMG;
    //                gun.mDMG = Random.Range(3, 8);
    //                gun.mFireRate = Random.Range(.1f, .25f);
    //                gun.mMaxAmmo = Random.Range(120, 300);
    //                gun.mMaxMag = Random.Range(30, 60);
    //                gun.mRange = Random.Range(5, 25);
    //                gun.mReloadSpeed = Random.Range(1, 2);

    //                break;
    //            }
    //        case 2:
    //            {
    //                Gun gun = Instantiate(RifleDrop, position, transform.rotation);
    //                gun.mtype = Gun.WeaponClass.Rifle;
    //                gun.mDMG = Random.Range(10, 20);
    //                gun.mFireRate = Random.Range(.3f, .7f);
    //                gun.mMaxAmmo = Random.Range(100, 180);
    //                gun.mMaxMag = Random.Range(15, 50);
    //                gun.mRange = Random.Range(150, 250);
    //                gun.mReloadSpeed = Random.Range(1, 2);

    //                break;
    //            }
    //        case 3:
    //            {
    //                Gun gun = Instantiate(SniperDrop, position, transform.rotation);
    //                gun.mtype = Gun.WeaponClass.Sniper;
    //                gun.mDMG = Random.Range(50, 100);
    //                gun.mFireRate = Random.Range(1f, 3);
    //                gun.mMaxAmmo = Random.Range(30, 60);
    //                gun.mMaxMag = Random.Range(5, 10);
    //                gun.mRange = Random.Range(200, 300);
    //                gun.mReloadSpeed = Random.Range(5, 9);

    //                break;
    //            }
    //        default:
    //            break;
    //    }
    //}
}
