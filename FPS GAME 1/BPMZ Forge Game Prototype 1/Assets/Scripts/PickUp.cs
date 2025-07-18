using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] Gun gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickUp pickup = other.GetComponent<IPickUp>();

        if (pickup != null)
        {
            
            gun.ammoCurrent = gun.ammoMax;
            gun.magCurrent = gun.magMax;
            pickup.PickUpGun(gun);
            Destroy(gameObject);
        }
    }
}