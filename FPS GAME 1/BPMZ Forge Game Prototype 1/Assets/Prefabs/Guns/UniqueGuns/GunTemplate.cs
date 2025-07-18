using System.Collections;
using UnityEngine;


// rename "GunTemplate" to the gun's name
public class GunTemplate : Gun
{
     public override void shoot(LayerMask ignoreLayer, AudioSource audio)
    {
        //if (ammoCurrent > 0)
        //{
        //    ammoCurrent--;
        //  //  shootTimer = 0;
        //    RaycastHit hit;

        //    //First person view location
        //    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit,
        //            shootDistance,
        //            ~ignoreLayer))
        //    {
        //       // playerAudio.PlayOneShot(currentGun.shootingSound);
        //        IDamage dmg = hit.collider.GetComponent<IDamage>();

        //        if (dmg != null)
        //        {
        //            //Debug.Log(equipGun.mDMG);
        //            dmg.takeDamage(shootDMG);
        //        }
        //    }
        //}
        //else
        //{
        //   // playerAudio.PlayOneShot(currentGun.emptyShotSound);
        //}
    }

    public override IEnumerator reload(AudioSource audio)
    {
        //audio.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(reloadTime);
        //if (magCurrent < magMax && ammoCurrent > 0)
        //{
        //    if (magMax - magCurrent <= ammoCurrent)
        //    {
        //        ammoCurrent -= magMax - magCurrent;
        //        magCurrent = magMax;
        //    }
        //    else
        //    {
        //        magCurrent += ammoCurrent;
        //        ammoCurrent = 0;


        //    }
        //}
        //gameManager.instance.updateAmmoPanel();
    }
}
