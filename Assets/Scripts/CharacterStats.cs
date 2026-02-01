using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
        countShoot++;
        if (countShoot >= 5)
        {
            isAggressive = true;
        }

    }

}