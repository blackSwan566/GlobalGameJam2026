using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Hier ziehst du dein blaues Prefab rein
    public int numberOfEnemies = 10; // Wie viele Gegner sollen erscheinen?
    public float spawnRange = 20f;   // Bereich, in dem sie verteilt werden

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Erzeuge eine zufällige Position auf dem Boden (Y bleibt 0.5 oder 1)
            Vector3 randomPos = new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0.5f,
                Random.Range(-spawnRange, spawnRange)
            );

            // Der magische Befehl: Erstelle eine Kopie des Prefabs an dieser Stelle
            Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        }
    }
}