using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GunManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GunManager instance;

    public List<GameObject> AllGuns;
    public List<GameObject> AllItems;
    
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    public void DropRandomGun(Transform pos)
    {
        int index = Random.Range(0, AllGuns.Count);
        Vector3 newposition = new Vector3(pos.transform.position.x, pos.transform.position.y + 1, pos.transform.position.z);
        Instantiate(AllGuns[index], newposition, pos.rotation);
    }

    public void DropRandomItem(Transform pos)
    {
        int index = Random.Range(0, AllItems.Count);
        Vector3 newposition = new Vector3(pos.transform.position.x, pos.transform.position.y + 1, pos.transform.position.z);
        Instantiate(AllItems[index], newposition, pos.rotation);
    }

}
