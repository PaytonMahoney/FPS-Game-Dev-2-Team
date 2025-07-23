using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{

    public static InteractionPromptUI instance;

    [SerializeField] private GameObject uiObject;
    [SerializeField] private TMP_Text promptText;

    void Awake()
    {
        Debug.Log("Awake is running!");

        instance = this;
        Debug.Log("✅ Prompt UI is initialized!");
        Debug.Log("InteractionPromptUI loaded.");
        Hide(); // Make sure it's hidden at start
    }

    public void Show(string message)
    {
        promptText.text = message;
        uiObject.SetActive(true);
    }

    public void Hide()
    {
        uiObject.SetActive(false);
    }
}
