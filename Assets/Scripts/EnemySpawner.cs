using UnityEngine;
using UnityEngine.AI; // WICHTIG: Damit NavMesh erkannt wird

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;    // Dein Gegner-Prefab
    public Transform playerTransform; // Dein Player aus der Hierarchy
    public int numberOfEnemies = 10;   // Anzahl der Gegner
    public float spawnRange = 20f;      // Radius

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // 1. Zufällige Position berechnen
            Vector3 randomPos = new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0.5f,
                Random.Range(-spawnRange, spawnRange)
            ) + transform.position;

            // 2. Den Gegner erstellen und in einer Variable speichern
            GameObject newEnemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);

            // 3. Dem neuen Gegner sagen, wer der Player ist
            EnemyAI ai = newEnemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.player = playerTransform;
            }
        }
    }
}


    