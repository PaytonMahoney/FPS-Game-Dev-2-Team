using UnityEngine;
[CreateAssetMenu]
public class OneJump : Item
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Activate()
    {

    }
    public override void Continuous()
    {

    }

    public override void OnPickup()
    {
        gameManager.instance.playerScript.jumpMax++;
    }

    public override void Deactivate() { }
}
