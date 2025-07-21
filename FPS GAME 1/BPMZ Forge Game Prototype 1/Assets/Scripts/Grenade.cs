using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    [SerializeField] int speed;
    [SerializeField] int upwardSpeed;
    [SerializeField] int destroyTime;

    [SerializeField] GameObject explosion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body.linearVelocity = transform.forward * speed + (transform.up * upwardSpeed);
        StartCoroutine(explode());
        
    }

    IEnumerator explode()
    {
        yield return new WaitForSeconds(destroyTime);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
