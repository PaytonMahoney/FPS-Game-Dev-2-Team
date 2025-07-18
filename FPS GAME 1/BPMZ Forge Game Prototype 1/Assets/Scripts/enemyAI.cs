using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Threading;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform shootPos2;
    [SerializeField] Transform headPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int HP;
    [SerializeField] int fov;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int roamDis;
    [SerializeField] int roamPauseTime;

    //Gun pistolDrop, SMGDrop, RifleDrop, SniperDrop;
    [SerializeField] int dropChance;

    Transform player;
    AudioSource shootingSound;

    Color colorOrg;
    float shootTimer;
    float angleToPlayer;
    float roamTime;
    float agentStopDisOrig;
    Vector3 playerDir;
    Vector3 startPos;
    bool playerInTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrg = model.material.color;
        startPos = transform.position;
        agentStopDisOrig = agent.stoppingDistance;
        player = GameObject.FindWithTag("Player").transform;
        shootingSound = GetComponent<AudioSource>();
        gameManager.instance.enemyCount++;
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
        agent.stoppingDistance = 0;
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
                shootTimer += Time.deltaTime;
                if (shootTimer >= shootRate)
                {
                    shoot();
                }
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
        if (HP <= 0)
        {
            if (Random.Range(0, 100) <= dropChance)
            {
                GunManager.instance.DropRandomGun(transform);
                //DropRandomGun(); 
            }
            Destroy(gameObject);
            gameManager.instance.enemyCount--;
            Debug.Log(gameManager.instance.enemyCount);
            if (gameManager.instance.enemyCount <= 0)
            {
                gameManager.instance.youWin();
            }
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.green;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrg;
    }

    void shoot()
    {
        shootTimer = 0;
        Vector3 directionToPlayer = (GameObject.FindWithTag("Player").transform.position - shootPos.position).normalized;

        GameObject bullet1 = Instantiate(bullet, shootPos.position, Quaternion.identity);
        bullet1.GetComponent<BulletMovement>().SetDirection(directionToPlayer);
        shootingSound.Play();

        Vector3 directionToPlayer2 = (GameObject.FindWithTag("Player").transform.position - shootPos2.position).normalized;

        GameObject bullet2 = Instantiate(bullet, shootPos2.position, Quaternion.identity);
        bullet2.GetComponent<BulletMovement>().SetDirection(directionToPlayer2);
        
        shootingSound.Play();

    }

    void DropARandomGun()
    {
        int num = Random.Range(0, 4);
        switch (num)
        {
            case 0:
                {
                    Gun gun = Instantiate(GunManager.instance.pistolDrop, transform.position, transform.rotation);
                    gun.mtype = Gun.WeaponClass.Pistol;
                    gun.mDMG = Random.Range(1, 10);
                    gun.mFireRate = Random.Range(.25f, 2);
                    gun.mMaxAmmo = Random.Range(45, 90);
                    gun.mMaxMag = Random.Range(5, 15);
                    gun.mRange = Random.Range(50, 150);
                    gun.mReloadSpeed = Random.Range(3, 6);

                    break;
                }
            case 1:
                {
                    Gun gun = Instantiate(GunManager.instance.SMGDrop, transform.position, transform.rotation);
                    gun.mtype = Gun.WeaponClass.SMG;
                    gun.mDMG = Random.Range(3, 10);
                    gun.mFireRate = Random.Range(.05f, .5f);
                    gun.mMaxAmmo = Random.Range(300, 750);
                    gun.mMaxMag = Random.Range(30, 60);
                    gun.mRange = Random.Range(40, 120);
                    gun.mReloadSpeed = Random.Range(2, 5);

                    break;
                }
            case 2:
                {
                    Gun gun = Instantiate(GunManager.instance.RifleDrop, transform.position, transform.rotation);
                    gun.mtype = Gun.WeaponClass.Rifle;
                    gun.mDMG = Random.Range(10, 20);
                    gun.mFireRate = Random.Range(.2f, 2);
                    gun.mMaxAmmo = Random.Range(300, 450);
                    gun.mMaxMag = Random.Range(15, 40);
                    gun.mRange = Random.Range(150, 250);
                    gun.mReloadSpeed = Random.Range(4, 8);

                    break;
                }
            default:
                {
                    Gun gun = Instantiate(GunManager.instance.SniperDrop, transform.position, transform.rotation);
                    gun.mtype = Gun.WeaponClass.Sniper;
                    gun.mDMG = Random.Range(50, 100);
                    gun.mFireRate = Random.Range(2f, 5);
                    gun.mMaxAmmo = Random.Range(30, 60);
                    gun.mMaxMag = Random.Range(5, 10);
                    gun.mRange = Random.Range(200, 300);
                    gun.mReloadSpeed = Random.Range(5, 9);

                    break;
                }
        }
    }
}