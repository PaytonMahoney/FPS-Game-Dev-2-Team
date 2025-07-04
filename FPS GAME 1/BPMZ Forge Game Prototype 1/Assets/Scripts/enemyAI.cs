using UnityEngine;
using System.Collections;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;


    [SerializeField] int HP;

    Color colorOrg;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrg = model.material.color;
        //Notify Manager that this enemy is in the level
        gameManager.instance.enemyCount += 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Needed so you can implement Damage
    public void takeDamage(int amount)
    {
        HP -= amount;


        if (HP <= 0)
        {
            //Keeping track of the enemy count 
            gameManager.instance.enemyCount -= 1;
            Destroy(gameObject);
        }
        else
        {
            //Only will flash if they're still alive
            StartCoroutine(flashRed());
        }
    }

    //FeedBack to player 
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrg;
    }
}
