using UnityEngine;
using UnityEngine.SceneManagement;

public class menuHandling : MonoBehaviour
{

    public void OnStartButton()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

}
