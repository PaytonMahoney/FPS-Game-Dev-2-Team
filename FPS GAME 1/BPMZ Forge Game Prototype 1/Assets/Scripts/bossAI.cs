using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class bossAI : MonoBehaviour, IDamage
{
    [SerializeField] private string[] bossNames;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform[] shootPositions;
    [SerializeField] Transform headPos;
    [SerializeField] GameObject[] bullets;
    [SerializeField] float[] shootRates;
    [SerializeField] int HP;
    private int maxHP;
    [SerializeField] int fov;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int roamDis;
    [SerializeField] int roamPauseTime;
    [SerializeField] private int levelNum;
    private int maxPhase;
    private int currentPhase;
    private bool canShoot;

    //Gun pistolDrop, SMGDrop, RifleDrop, SniperDrop;
    [SerializeField] int dropChance;

    Transform player;
    AudioSource soundManager;
    [SerializeField] private AudioClip[] shootingSoundClips;
    [SerializeField] private AudioClip[] bossMusicClipsPerPhase;
    [SerializeField] private int[] bulletDMG;

    Color colorOrg;
    float[] shootTimers = new float[3];
    float angleToPlayer;
    float roamTime;
    float agentStopDisOrig;
    Vector3 playerDir;
    Vector3 startPos;
    bool playerInTrigger;
    EnemyKillCounter counter;
    [SerializeField] GameObject teleporterPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //colorOrg = model.material.color;
        startPos = transform.position;
        agentStopDisOrig = agent.stoppingDistance;
        player = GameObject.FindWithTag("Player").transform;
        soundManager = GetComponent<AudioSource>();
        levelNum = SceneManager.GetActiveScene().buildIndex;
        counter = FindFirstObjectByType<EnemyKillCounter>();

        //gameManager.instance.enemyCount++;
        HP += (levelNum - 1) * 200;
        maxHP = HP;
        currentPhase = 1;
        //nextPhase();
        soundManager.loop = true;
        gameManager.instance.bossNameText.SetText(bossNames[currentPhase-1]);
        canShoot = true;
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].GetComponent<damagetypes>().damageAmount = bulletDMG[i];
            bullets[i].GetComponent<damagetypes>().speed = 40;
        }

        maxPhase = ((int)(levelNum / 2)) + 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 0.01f)
        {
            roamTime += Time.deltaTime;
        }
        if (playerInTrigger && canSeePlayer())
        {
            roamCheck();
        }
        else if (!playerInTrigger)
        {
            roamCheck();
        }
    }

    void roamCheck()
    {
        if (roamTime >= roamPauseTime && agent.remainingDistance < 0.01)
        {
            roam();
        }
    }

    void roam()
    {
        roamTime = 0;
        agent.stoppingDistance = 24;
        Vector3 ranPos = Random.insideUnitSphere * roamDis;
        ranPos += startPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDis, 1);
        agent.SetDestination(hit.position);
    }

    bool canSeePlayer()
    {
        playerDir = GameObject.FindWithTag("Player").transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {
                shoot();
                agent.SetDestination(GameObject.FindWithTag("Player").transform.position);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }
                agent.stoppingDistance = agentStopDisOrig;
                return true;
            }
            else
            {
                agent.stoppingDistance = 0;
                return false;
            }
        }
        else
        {
            agent.stoppingDistance = 0;
            return false;
        }
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            gameManager.instance.bossHPUI.SetActive(true);
            soundManager.clip = bossMusicClipsPerPhase[currentPhase-1];
            soundManager.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            agent.stoppingDistance = 0;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updateBossUI();
        if (HP <= 0)
        {
            if (currentPhase < maxPhase)
            {
                //Debug.Log("HERE");
                nextPhase();
            }
            else
            {

                
                  counter.EnemyKilled();

                


                gameManager.instance.bossHPUI.SetActive(false);
                Debug.Log("💀 Boss defeated. Spawning teleporter!");
                // 💥 Spawn teleporter just before destroying boss
                Vector3 spawnPos = transform.position + Vector3.up * 0.5f + transform.forward * 3f;
             
                Instantiate(teleporterPrefab, spawnPos, Quaternion.identity);



                Destroy(gameObject);
                // Destroy(gameObject);
                //gameManager.instance.bossHPUI.SetActive(false);

                // needs to spawn teleporter or game over/you win
            }
        }
    }

    void shoot()
    {
       for (int i = 0; i < shootTimers.Length; i++)
        {
            //Debug.Log(shootTimers.Length);
            shootTimers[i] += Time.deltaTime;
            if (shootTimers[i] >= shootRates[i])
            {
                //Debug.Log(i);
                if(canShoot)
                    shootWeapon(i);
            }
        }
    }

    void shootWeapon(int weapon)
    {
        shootTimers[weapon] = 0;
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        playerPosition.y += 1;
        Vector3 directionToPlayer = (playerPosition - shootPositions[weapon].position).normalized;
        GameObject bullet = Instantiate(bullets[weapon], shootPositions[weapon].position, Quaternion.identity);
        bullet.GetComponent<BulletMovement>().SetDirection(directionToPlayer);
        soundManager.PlayOneShot(shootingSoundClips[weapon]);
        //Debug.Log(bullet.name);
    }
    
    void updateBossUI()
    {
        gameManager.instance.bossHPBar.fillAmount = (float)HP / maxHP;
    }

    void nextPhase()
    {
        currentPhase++;
        canShoot = false;
        if(soundManager.isPlaying)
            soundManager.Stop();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameManager.instance.bossHPUI.SetActive(false);
        maxHP *= 2;
        HP = maxHP;
        //Debug.Log("NEXT PHASE");
        for (int i = 0; i < 3; i++)
        {
            bullets[i].GetComponent<damagetypes>().damageAmount += 1;
            bullets[i].GetComponent<damagetypes>().speed += 40;
            for (int j = 0; j < shootRates.Length; j++)
            {
                shootRates[j] *= 0.75f;
            }
        }
        updateBossUI();
        StartCoroutine(preparePhase());
    }
    
    IEnumerator preparePhase()
    {
        gameManager.instance.bossNameText.SetText(bossNames[currentPhase-1]);
        yield return new WaitForSeconds(3f);
        Debug.Log("preparePhase");
        gameManager.instance.bossHPUI.SetActive(true);
        //gameObject.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        soundManager.clip = bossMusicClipsPerPhase[currentPhase-1];
        soundManager.Play();
        canShoot = true;
    }
}
