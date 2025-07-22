using UnityEngine;
[CreateAssetMenu]
public class HPup : Item
{
    [SerializeField] int HPModifier;
    public override void Activate()
    {
        
    }
    public override void Continuous()
    {
       
    }

    public override void OnPickup()
    {
        gameManager.instance.playerScript.maxHP += HPModifier;
        gameManager.instance.playerScript.Heal(HPModifier);
        gameManager.instance.playerScript.updatePlayerUI();
    }

    public override void Deactivate() { }
}
