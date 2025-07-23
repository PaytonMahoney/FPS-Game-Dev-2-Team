using UnityEngine;
using UnityEngine.SceneManagement;
public class EnemyKillCounter : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int enemiesToKill = 10;
    private int currentKills = 0;

    [Header("Teleporter Stuff")]
    public GameObject teleporterPrefab;
    public Transform spawnLocation;

    private bool spawnedTeleporter = false;

    public void EnemyKilled()
    {
        currentKills++;
        Debug.Log("Enemy killed! Total: " + currentKills);

        if (currentKills >= enemiesToKill && !spawnedTeleporter)
        {
            SpawnTeleporter();
        }
    }

    void SpawnTeleporter()
    {
        Instantiate(teleporterPrefab, spawnLocation.position, Quaternion.identity);
        Debug.Log("✅ Teleporter spawned after killing enough enemies!");
        spawnedTeleporter = true;
    }
}
