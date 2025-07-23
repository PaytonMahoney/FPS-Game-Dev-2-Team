using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    public GameObject buttonInteract;
    
    public Image playerHPBar;
    public TMP_Text playerHPText;
    public GameObject playerDMGPanel;
    public GameObject playerHealPanel;
    public bool isPaused;
    public GameObject player;
    public playerController playerScript;
    public GameObject boss;
    public TMP_Text playerAmmoText;
    
    public Image activeItemImage;
    public TMP_Text activeItemRechargeText;
    public Image activeItemRechargePanel;
    public Image activeItemInUse;
    
    float timescaleOriginal;
    bool bossDead;
    public int enemyCount;
    
    [SerializeField] public GameObject bossHPUI;
    [SerializeField] public Image bossHPBar;
    [SerializeField] public TMP_Text bossNameText;
    [SerializeField] public GameObject teleporter;
    
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timescaleOriginal = Time.timeScale;
        enemyCount = 0;
        activeItemImage.enabled = false;
        activeItemRechargePanel.enabled = false;
        activeItemRechargeText.enabled = false;
        activeItemInUse.enabled = false;
        boss =  GameObject.FindWithTag("MainBoss");
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
        if (menuActive != null)
        {
            menuActive.SetActive(false);
        }
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

    public void updateAmmoPanel()
    {
        playerAmmoText.text = playerScript.currentGun.gunName + "\n" + playerScript.currentGun.magCurrent.ToString() + " / " + playerScript.currentGun.ammoCurrent;
    }
    public void loadLevel(int lvl)
    {
        SceneManager.LoadScene(lvl);
        gameManager.instance.stateUnpause();
    }

}
