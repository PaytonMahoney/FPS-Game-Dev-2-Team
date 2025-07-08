using UnityEngine;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;


    public bool isPaused;
    public GameObject player;
    public playerController playerScript;
<<<<<<< Updated upstream
=======
    public GameObject mainBoss;
   
    
>>>>>>> Stashed changes

    float timescaleOrig;

    //Boss Variable // Different AI
    int bossDead;

    public int enemyCount;

    void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
<<<<<<< Updated upstream
        timescaleOrig = Time.timeScale;
=======
        timescaleOriginal = Time.timeScale;
        
       
        
        
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        
=======
        // Pausing
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }

        //if (bossScript. <= 0)
        //{
        //    youWin();
        //}
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timescaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
>>>>>>> Stashed changes
    }
}
