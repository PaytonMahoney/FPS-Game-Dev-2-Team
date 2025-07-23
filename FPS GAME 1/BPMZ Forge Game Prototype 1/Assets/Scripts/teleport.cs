using UnityEngine;
using UnityEngine.SceneManagement;

public class teleport : MonoBehaviour
{
    Scene currScene;
    
    private void Start()
    {
        currScene = SceneManager.GetActiveScene();
        Debug.Log("." + currScene.name + ".");
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int nextScene = currScene.buildIndex + 1;

            if (nextScene < SceneManager.sceneCountInBuildSettings)
            {
                gameManager.instance.loadLevel(nextScene);
            }
            else
            {
                Debug.LogWarning("⚠️ No next scene to load! You reached the end of the scene list.");
            }
        }
    }

}
