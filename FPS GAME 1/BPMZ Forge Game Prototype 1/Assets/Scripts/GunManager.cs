using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

public class GunManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GunManager instance;

    public List<GameObject> AllGuns;
    
    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Transform player = GameObject.FindWithTag("Player").transform;
            DropRandomGun(player);
            Debug.Log("💥 Debug gun dropped at player location!");
        }
    }


    // Update is called once per frame
    public void DropRandomGun(Transform pos)
    {
        int index = Random.Range(0, AllGuns.Count);
        Instantiate(AllGuns[index], pos.position, pos.rotation);
    }

    
}
