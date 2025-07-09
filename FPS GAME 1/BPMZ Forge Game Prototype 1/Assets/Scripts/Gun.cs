using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading;
using System.Numerics;

public class Gun : MonoBehaviour
{
    public enum WeaponClass
    {
        Pistol,
        SMG,
        Rifle,
        Sniper,
    } 
    [SerializeField] public WeaponClass mtype;
    [SerializeField] public int mDMG;
    [SerializeField] public float mFireRate;
    [SerializeField] public int mMaxAmmo;  
    [SerializeField] public int mMaxMag;
    [SerializeField] public int mRange;
    [SerializeField] public int mReloadSpeed;

    public int currentAmmo;
    public int currentMag;

    private void Start()
    {
        currentAmmo = mMaxAmmo;
        currentMag = mMaxMag;
    }

    public Gun(int type, int DMG = 0, float FireRate = 0, int MaxAmmo = 0,int MaxMag = 0,int Range = 0,int ReloadSpeed = 0)
    {
        switch (type) {
            case 0:
                {
                    mtype = WeaponClass.Pistol;
                    break;
            }
            case 1: 
                {
                    mtype = WeaponClass.SMG;
                    break;
            }
            case 2: 
                { 
                    mtype = WeaponClass.Rifle;
                    break;
            }
            default: 
                {
                    mtype= WeaponClass.Sniper;
                    break;
            }
        }
        mDMG = DMG;
        mFireRate = FireRate;
        mMaxAmmo = MaxAmmo;
        mMaxMag = MaxMag;
        mRange = Range;
        mReloadSpeed = ReloadSpeed;
        currentAmmo = MaxAmmo;
        currentMag = mMaxMag;
    }
    
  

    public IEnumerator ReloadGun()
    {
        if (currentMag < mMaxMag && currentAmmo > 0)
        {
            yield return new WaitForSeconds(mReloadSpeed);
            if (currentAmmo > mMaxMag - currentMag) 
            {
            currentAmmo -= mMaxMag - currentMag;
                currentMag = mMaxMag;
            }
            else
            {
                currentMag += currentAmmo;
                currentAmmo = 0;
            }


                
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
            if (other.gameObject.CompareTag("Player"))
            {
          //  Instantiate(other.GetComponent<playerController>().equipGun,transform.position,transform.rotation);
            other.GetComponent<playerController>().equipGun = this;
                Destroy(gameObject);
            }
        
        
    }


}
