using UnityEngine;

public abstract class Item : ScriptableObject
{
    enum itemType
    {
        Active,
        SingleUseActive,
        Passive,
    }
    
    [SerializeField] itemType type;
    public GameObject model;
    
    public abstract void OnPickup();
    public abstract void Continuous();

    public abstract void Activate();


    
}
