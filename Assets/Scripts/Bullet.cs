using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Wir prüfen, ob das getroffene Objekt ein Gegner ist
        if (other.CompareTag("Enemy"))
        {
            // Wir holen uns die Stats, um Leben abzuziehen und Treffer zu zählen
            CharacterStats stats = other.GetComponent<CharacterStats>();
            if (stats != null)
            {
                stats.TakeDamage(1); // Hier wird jetzt alles korrekt verarbeitet
            }

            // Die Kugel verschwindet beim Aufprall
            Destroy(gameObject);
        }
    }
}