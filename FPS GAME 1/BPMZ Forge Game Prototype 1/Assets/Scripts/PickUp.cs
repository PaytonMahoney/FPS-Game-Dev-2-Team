using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] GunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickUp pickup = other.GetComponent<IPickUp>();

        if (pickup != null)
        {
            pickup.GetGunStats(gun);
            gun.ammoCurrent = gun.ammoMax;
            Destroy(gameObject);
        }
    }
}