using UnityEngine;

public class PickUp : MonoBehaviour
{
    enum pickupType
    {
        Gun,
        Item,
        Consumable
    }
    [SerializeField] pickupType type;
    [SerializeField] Gun gun;
    [SerializeField] Item item;
    [SerializeField] bool spin;
    private void Update()
    {
        if (spin)
        {
            transform.Rotate(0, 1, 0);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        IPickUp pickup = other.GetComponent<IPickUp>();

        if (pickup != null && type == pickupType.Gun)
        {
            
            gun.ammoCurrent = gun.ammoMax;
            gun.magCurrent = gun.magMax;
            pickup.PickUpGun(gun);
            Destroy(gameObject);
        }
        else if (pickup != null && type == pickupType.Item)
        {
            pickup.PickUpItem(item);
            Destroy(gameObject);
        }

    }
}