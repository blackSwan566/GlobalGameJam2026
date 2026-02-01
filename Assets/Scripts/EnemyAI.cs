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

        // NATÜRLICHES VERHALTEN: Zufällige Werte für jeden Gegner
        agent.speed = Random.Range(3f, 7f);          // Unterschiedliches Tempo
        agent.acceleration = Random.Range(4f, 10f);  // Unterschiedliches Anlaufen
        agent.stoppingDistance = Random.Range(1.5f, 4f); // Halten in unterschiedlichem Abstand an

        // AUTOMATISCHE SUCHE: Falls im Inspektor nicht zugewiesen
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // NavMesh-Sicherheit: Agent auf das Mesh zwingen
        if (agent != null && agent.isActiveAndEnabled && !agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    void Update()
    {
        if (isDead || player == null || agent == null) return;

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }

        if (anim != null)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null) agent.isStopped = true;
        gameObject.SetActive(false);
    }
}