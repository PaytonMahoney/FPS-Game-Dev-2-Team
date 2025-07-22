using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class damagetypes : MonoBehaviour
{
    
    enum damageType
    {
        moving, //bullets
        stationary, //Spike walls, etc.
        DOT, //Damage over time
        homing,
        explosion,

        healing //Heals instead of damaging
    }

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate; //for DOT effects
    [SerializeField] int speed; //moving or homing type 
    [SerializeField] float destroyTime; //if moving or homing type doesn't hit  

    bool isDamaging; //for DOT
    
    void Start()
    {
        if (type == damageType.moving || type == damageType.homing || type == damageType.explosion)
        {
            Destroy(gameObject, destroyTime); //Destroy object by destroy time so memory isn't taken

            if (type == damageType.moving)
            {
                rb.linearVelocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
            }
        }
    }

    
    void Update()
    {
        if(type == damageType.homing)
        {
            rb.linearVelocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;


        //checks to see if the thing collied can be damaged 
        if (type != damageType.healing)
        {
            IDamage dmg = other.GetComponent<IDamage>();




            //Every other type other than DOT
            if (dmg != null && type != damageType.DOT)
            {
                dmg.takeDamage(damageAmount);
            }

            if (type == damageType.moving || type == damageType.homing)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            IHeal heal = other.GetComponent<IHeal>();
            if (heal != null)
            {
                if (heal.Heal(damageAmount))
                {
                    Destroy(gameObject);
                }
            }
        }
            
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

}
