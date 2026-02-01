using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Wichtig!

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform playerTransform;
    public int numberOfEnemies = 10;
    public float spawnRange = 20f;

    [Header("Spawn Einstellungen")]
    public float spawnDelay = 1.5f; // Zeit zwischen den Spawns

    void Start()
    {
        // Sicherstellen, dass die Coroutine nur einmal gestartet wird
        StopAllCoroutines();
        StartCoroutine(SpawnEnemiesRoutine());
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        // 1. Einen winzigen Moment warten, damit nicht alles im ersten Frame passiert
        yield return null;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Position berechnen
            Vector3 randomPos = new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0.5f,
                Random.Range(-spawnRange, spawnRange)
            ) + transform.position;

            // Gegner instanziieren
            GameObject newEnemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);

            // AI Ziel zuweisen
            EnemyAI ai = newEnemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.player = playerTransform;
            }

            Debug.Log("Gegner " + (i + 1) + " gespawnt. Warte " + spawnDelay + " Sekunden...");

            // WARTEN: Das ist die entscheidende Zeile
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}