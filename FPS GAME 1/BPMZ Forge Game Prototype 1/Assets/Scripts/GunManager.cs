using UnityEngine;

public class GunManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GunManager instance;

    [SerializeField] public Gun pistolDrop, SMGDrop, RifleDrop, SniperDrop;
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropRandomGun(Transform location)
    {
        int num = Random.Range(0, 4);
        switch (num)
        {
            case 0:
                {
                    Gun gun = Instantiate(pistolDrop, location.position, location.rotation);
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
                    Gun gun = Instantiate(SMGDrop, location.position, location.rotation);
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
                    Gun gun = Instantiate(RifleDrop, location.position, location.rotation);
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
                    Gun gun = Instantiate(SniperDrop, location.position, location.rotation);
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
