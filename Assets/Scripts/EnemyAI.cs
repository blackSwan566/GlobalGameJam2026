using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    private bool isDead = false;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }

        if (anim != null)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    // Wird aufgerufen, wenn die Kugel trifft
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Option A: Sofort ausstellen
        gameObject.SetActive(false);

        // Option B: Falls du ihn erst nach 0.5 Sek ausstellen willst (für Sound o.ä.)
        // Invoke("Deactivate", 0.5f); 
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}