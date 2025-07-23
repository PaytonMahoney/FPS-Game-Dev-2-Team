using UnityEngine;

public class door : MonoBehaviour
{
    
    [SerializeField] private GameObject doorModel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        
        if (other.CompareTag("Player"))
        {
            doorModel.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;
        
        if (other.CompareTag("Player"))
        {
            doorModel.SetActive(true);
        }
    }
}
