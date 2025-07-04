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

    float timescaleOrig;

    //Boss Variable // Different AI
    int bossDead;

    public int enemyCount;

    void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timescaleOrig = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
