using UnityEngine;

public class Barrel : MonoBehaviour, IDamage
{

    [SerializeField] int hp;

    [SerializeField] GameObject explosionEffect;

    [SerializeField] private AudioClip explosionSound;

    public int getHP()
    {
        return hp;
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            gameManager.instance.playerScript.GetGunAudioSource().PlayOneShot(explosionSound);
            Destroy(gameObject);
        }
    }

}
