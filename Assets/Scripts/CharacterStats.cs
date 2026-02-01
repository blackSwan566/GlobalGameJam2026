using UnityEngine;
using UnityEngine.AI;

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
        countShoot++; // WICHTIG: Erhöht den Zähler bei jedem Treffer

        // Debug-Ausgabe in der Konsole zur Kontrolle
        Debug.Log(gameObject.name + " getroffen! Leben: " + lifes + " | Schüsse: " + countShoot);

        // Animationen abspielen (sofern vorhanden)
        if (anim != null)
        {
            anim.SetTrigger("Hit");
            anim.SetTrigger("TakeMask");
        }

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
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " ist gestorben!");

        if (anim != null) anim.SetBool("Die", true);

        // KI stoppen
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.isStopped = true;

        // Das Objekt nach einer kurzen Verzögerung (für die Animation) deaktivieren
        Invoke("DeactivateObject", 0.5f);
    }

    void DeactivateObject()
    {
        gameObject.SetActive(false);
    }

    void ShootCount()
    {
        if (isAggressive) return;
        isAggressive = true;

        Debug.Log(gameObject.name + " wird aggressiv!");
        if (anim != null) anim.SetBool("Shoot", true);

        // Aggressives Verhalten: Gegner wird schneller statt anzuhalten
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed *= 2; // Verdoppelt das Tempo
            agent.angularSpeed *= 2; // Dreht sich schneller
        }
    }
}