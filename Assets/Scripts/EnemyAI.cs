using UnityEngine;
using UnityEngine.AI; 

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public float attackRange = 2f;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance > attackRange)
        {
            // Spieler verfolgen
            agent.SetDestination(player.position);
            anim.SetFloat("Speed", 1f);
        }
        else
        {
            // Angreifen, wenn nah genug
            agent.SetDestination(transform.position); // Stoppen
            anim.SetFloat("Speed", 0f);
            anim.SetTrigger("Attack");
        }
    }
}