using System.Collections;
using UnityEngine;

public class Adrenaline : Item
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public override void OnPickup()
    {
        currentCooldown = itemCooldown;
    }
    public override void Continuous()
    {

    }

    public override void Activate()
    {
        inUse = true;
        gameManager.instance.playerScript.DMGreduction += 25;
        gameManager.instance.playerScript.moveSpeed *= 2;
        
    }
    public override void Deactivate()
    {
        inUse = false;
        gameManager.instance.playerScript.DMGreduction -= 25;
        gameManager.instance.playerScript.moveSpeed /= 2;
    }

    
}
