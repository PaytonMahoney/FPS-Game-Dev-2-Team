using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu]
public class GrenadeLauncher : Gun
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public override void shoot(LayerMask ignoreLayer, AudioSource audio)
    {
        if (magCurrent > 0)
        {
            magCurrent--;
            //  shootTimer = 0;
            //RaycastHit hit;

            //First person view location
            //if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit,
            //        shootDistance,
            //        ~ignoreLayer))
            //{

            Instantiate(projectile, shootPOS.position, gameManager.instance.player.transform.rotation);

            { 
                audio.PlayOneShot(shootingSound);
                //IDamage dmg = hit.collider.GetComponent<IDamage>();

                //if (dmg != null)
                //{
                //    //Debug.Log(equipGun.mDMG);
                //    dmg.takeDamage(shootDMG);
                //}
            }
        }
        else
        {
            audio.PlayOneShot(emptyShotSound);
        }
    }

    public override IEnumerator reload(AudioSource audio)
    {
        audio.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(reloadTime);
        if (magCurrent < magMax && ammoCurrent > 0)
        {
            if (magMax - magCurrent <= ammoCurrent)
            {
                ammoCurrent -= magMax - magCurrent;
                magCurrent = magMax;
            }
            else
            {
                magCurrent += ammoCurrent;
                ammoCurrent = 0;


            }
        }
        gameManager.instance.updateAmmoPanel();
    }
}
