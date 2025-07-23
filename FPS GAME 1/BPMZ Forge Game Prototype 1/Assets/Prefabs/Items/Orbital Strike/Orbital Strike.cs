using System.Collections;
using UnityEngine;
[CreateAssetMenu]
public class OrbitalStrike : Item
{
    [SerializeField] GameObject Rocket;
    private Vector3 strikeLocation;
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
        strikeLocation = new Vector3(gameManager.instance.player.transform.position.x,gameManager.instance.player.transform.position.y + 50, gameManager.instance.player.transform.position.z);
    }
    public override void Deactivate()
    {
        inUse = false;
        Instantiate(Rocket, strikeLocation, Rocket.transform.rotation);
    }

    
}
