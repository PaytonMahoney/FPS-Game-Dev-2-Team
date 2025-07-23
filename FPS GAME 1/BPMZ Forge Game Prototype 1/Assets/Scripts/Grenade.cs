using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] public int speed;
    [SerializeField] public int upwardSpeed;
    [SerializeField] int destroyTime;
    [SerializeField] bool impact;

    [SerializeField] GameObject explosion;

    [SerializeField] private AudioClip explosionSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        body.linearVelocity = transform.forward * speed +  (transform.up * upwardSpeed);
        

        StartCoroutine(explode());
        
    }

    IEnumerator explode()
    {
        yield return new WaitForSeconds(destroyTime);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        
        if (impact)
        {
            if (other != null && !other.CompareTag("Player"))
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                gameManager.instance.playerScript.GetGunAudioSource().PlayOneShot(explosionSound);
                Destroy(gameObject);
            }
        }
    }
}
