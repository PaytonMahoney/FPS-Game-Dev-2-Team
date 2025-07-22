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
       if(other.CompareTag("Player"))
       {
            gameManager.instance.loadLevel(currScene.buildIndex + 1);
       }      
    }

}
