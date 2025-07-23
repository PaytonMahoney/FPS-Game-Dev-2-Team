using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonFunctions : MonoBehaviour
{

    [SerializeField] AudioClip uiClick;
    [SerializeField] AudioSource uiSource;

    public void resume()
    {
        
        uiSource.PlayOneShot(uiClick);
        
        gameManager.instance.stateUnpause();
       

    }

    public void restart()
    {
        SceneManager.LoadScene(1);
        gameManager.instance.stateUnpause();

    }

    public void quit()
    {
  #if !UNITY_EDITOR
        Application.Quit();
  #else
         
  UnityEditor.EditorApplication.isPlaying = false;

  #endif
    }

    public void respawn()
    {
        
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Just in case time was frozen
        gameManager.instance.stateUnpause();

        // Optional: If your GameManager handles UI, it should hide Lose Menu automatically
    }

   

}
