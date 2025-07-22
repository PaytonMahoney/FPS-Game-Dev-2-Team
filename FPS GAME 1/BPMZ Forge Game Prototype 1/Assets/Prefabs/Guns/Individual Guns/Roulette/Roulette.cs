using UnityEngine;
using System.Collections;
//[CreateAssetMenu(fileName = "Roulette", menuName = "ScriptableObjects/Pistols/Roulette", order = 1)]


public class Roulette : Gun
{
   

    public override void shoot(LayerMask ignoreLayer, AudioSource audio)
    {
        //if (gunAudio == null)
        //{
        //    gunAudio = gunModel.GetComponent<AudioSource>();
        //}
        if (magCurrent > 0)
        {
            
            magCurrent--;
            //  shootTimer = 0;
            RaycastHit hit;
            if (Random.Range(0, magCurrent) == 0) {
                //First person view location
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit,
                        shootDistance,
                        ~ignoreLayer))
                {
                    audio.PlayOneShot(shootingSound);
                    IDamage dmg = hit.collider.GetComponent<IDamage>();

                    if (dmg != null)
                    {
                        //Debug.Log(equipGun.mDMG);
                        dmg.takeDamage(shootDMG);
                    }
                }
                magCurrent = 0;
            }
            else
            {
                // playerAudio.PlayOneShot(currentGun.emptyShotSound);
                audio.PlayOneShot(emptyShotSound);
                
            }
        }
        else
        {
            // playerAudio.PlayOneShot(currentGun.emptyShotSound);
            audio.PlayOneShot(emptyShotSound);
        }
    }

    public override IEnumerator reload(AudioSource audio)
    {
        audio.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(reloadTime);

        magCurrent = 6;
        ammoCurrent--;


        gameManager.instance.updateAmmoPanel();
    }    
    
}
