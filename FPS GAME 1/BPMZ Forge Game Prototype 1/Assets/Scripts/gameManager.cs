using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public Image playerHPBar;
    public GameObject playerDMGPanel;
    public bool isPaused;
    public GameObject player;
    public playerController playerScript;
    public GameObject mainBoss;

    float timescaleOriginal;
    bool bossDead;
    public int enemyCount;

    void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");
        mainBoss = GameObject.FindWithTag("MainBoss");
        playerScript = player.GetComponent<playerController>();
        timescaleOriginal = Time.timeScale;
        
        
    }

    // Update is called once per frame
    void Update()
    {
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

        //if (mainBoss.HP <= 0)
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
    }
}
