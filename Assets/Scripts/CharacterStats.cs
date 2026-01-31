using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int health = 100;
    public Animator anim;
    public bool isDead = false;

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        anim.SetTrigger("Hit");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return; // Sicherheitshalber, damit Die() nur einmal läuft
        isDead = true;

        anim.SetBool("Die", true);

        // 1. Bewegung stoppen (KI)
        if (GetComponent<UnityEngine.AI.NavMeshAgent>())
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        // 2. Bewegung stoppen (Player)
        if (GetComponent<CharacterController>())
            GetComponent<CharacterController>().enabled = false;

        // 3. WICHTIG: Wenn es ein Gegner ist, nach 5 Sekunden löschen
        // Das hält deine Szene sauber, wenn du viele Gegner spawnst
        if (gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject, 5f);
        }
    }
}