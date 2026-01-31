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
        anim.SetTrigger("Hit"); // Spielt die "Hit" Animation ab

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetBool("Die", true); // Spielt die "Die" Animation ab

        // Verhindert, dass die KI oder der Player sich weiter bewegt
        if (GetComponent<UnityEngine.AI.NavMeshAgent>())
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        if (GetComponent<CharacterController>())
            GetComponent<CharacterController>().enabled = false;
    }
}