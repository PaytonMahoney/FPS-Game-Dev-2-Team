using UnityEngine;
[CreateAssetMenu]
public class HPup : Item
{
    [SerializeField] int HPmodifier;
    public override void Activate()
    {
        
    }
    public override void Continuous()
    {
       
    }

    public override void OnPickup()
    {
        gameManager.instance.playerScript.maxHP += HPmodifier;
        gameManager.instance.playerScript.Heal(HPmodifier);
        gameManager.instance.playerScript.updatePlayerUI();
    }
}
