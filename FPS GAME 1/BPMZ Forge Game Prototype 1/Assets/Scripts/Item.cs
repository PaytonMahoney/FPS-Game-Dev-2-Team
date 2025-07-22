using UnityEngine;
using UnityEngine.UI;
public abstract class Item : ScriptableObject
{
    public enum itemType
    {
        Active,
        SingleUseActive,
        Passive,
    }
    [SerializeField] public Sprite icon;
    [SerializeField] public itemType type;
    //public GameObject model;
    [SerializeField] public int itemDuration;

    [SerializeField] public int itemCooldown;

    public float currentCooldown;
    public bool inUse;
    public abstract void OnPickup();
    public abstract void Continuous();

    public abstract void Activate();

    public abstract void Deactivate();
    
}
