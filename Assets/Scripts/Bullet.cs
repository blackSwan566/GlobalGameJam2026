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
        if (other.CompareTag("Enemy"))
        {
            CharacterStats stats = other.GetComponent<CharacterStats>();
            if (stats != null)
            {
                // Wir senden den Schaden an die Stats
                stats.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }
}