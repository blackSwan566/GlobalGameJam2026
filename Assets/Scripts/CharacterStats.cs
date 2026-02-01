using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int lifes = 5;
    public int countShoot = 0;
    public Animator anim;
    public bool isDead = false;
    public bool isAggressive = false;


    public void TakeDamage(int damage)
    {
        if (isDead) return;

        lifes -= damage;
        
        anim.SetTrigger("Hit");
        anim.SetTrigger("TakeMask");

        if (lifes <= 0)
        {
            Die();
        }

        if (countShoot >= 5)
        {
            ShootCount();
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
            Debug.Log("Enemy destroyed");
        }
    }

    void ShootCount()
    {
        if (isAggressive) return;

        anim.SetBool("Shoot", true);

        // 1. Bewegung stoppen (KI)
        if (GetComponent<UnityEngine.AI.NavMeshAgent>())
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        // 2. Bewegung stoppen (Player)
        if (GetComponent<CharacterController>())
            GetComponent<CharacterController>().enabled = false;

        Debug.Log("Character is aggressive maaaaan");
    }
}