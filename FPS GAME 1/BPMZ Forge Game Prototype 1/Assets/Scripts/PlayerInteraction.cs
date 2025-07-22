using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    private IInteractable currentInteractable;
    private string currentPromptMessage = "Press E to Talk";

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.red);

        Debug.Log("Interaction system is running");

        // Draw the ray for debugging in the Scene view
        Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.red);

        // Create the ray
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Cast the ray
        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            Debug.Log("Ray hit: " + hit.collider.name);  // Print what we hit

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                Debug.Log("Found IInteractable!");

                currentInteractable = interactable;
                InteractionPromptUI.instance.Show(currentPromptMessage);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    currentInteractable.Interact();
                }
            }
            else
            {
                Debug.Log("Hit object has no IInteractable");
                ClearInteraction();
            }
        }
        else
        {
            // Ray hit nothing
            ClearInteraction();
        }
    }

    void ClearInteraction()
    {
        if (currentInteractable != null)
        {
            currentInteractable = null;
            InteractionPromptUI.instance.Hide();
        }
    }
}

