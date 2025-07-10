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

    public Gun(int type)
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
    }
    
    public IEnumerator ReloadGun()
    {
        if (currentMag < mMaxMag && currentAmmo >= 0)
        {
            yield return new WaitForSeconds(mReloadSpeed);
            currentAmmo = mMaxAmmo;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
            if (other.gameObject.CompareTag("Player"))
            {
                gameManager.instance.GetComponent<playerController>().UpdatePlayerGun(this);
                gameObject.SetActive(false);
            }
        
    }
}
