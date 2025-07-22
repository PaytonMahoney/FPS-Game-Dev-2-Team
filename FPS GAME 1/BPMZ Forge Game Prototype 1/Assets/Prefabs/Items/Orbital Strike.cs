using UnityEngine;
[CreateAssetMenu]
public class OrbitalStrike : Item
{
    [SerializeField] GameObject Rocket;
    public override void OnPickup()
    {
        currentCooldown = itemCooldown;
    }
    public override void Continuous()
    {

    }

    public override void Activate()
    {
        

    }
    public override void Deactivate()
    {
        
    }
}
