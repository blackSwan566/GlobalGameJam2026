using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    private bool isDead = false;

    public GameObject Mask, MasksShattered;



    [Header("Damage")]
    public float damageRadius = 1.5f; // distance at which the enemy damages the player
    public int damageAmount = 1;      // how many lifes to remove
    public float damageCooldown = 1.0f; // seconds between damage ticks
    private float lastDamageTime = -Mathf.Infinity;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();

        // NATÜRLICHES VERHALTEN: Zufällige Werte für jeden Gegner
        agent.speed = Random.Range(3f, 7f);          // Unterschiedliches Tempo
        agent.acceleration = Random.Range(4f, 10f);  // Unterschiedliches Anlaufen
        agent.stoppingDistance = Random.Range(1.5f, 4f); // Halten in unterschiedlichem Abstand an

        anim.SetFloat("Run", agent.speed); // Unterschiedliche Animationsgeschwindigkeit

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

        // Check proximity and damage player if in range (with cooldown)
        TryDamagePlayerInRange();
    }

    void TryDamagePlayerInRange()
    {
        if (player == null) return;
        // distance check
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= damageRadius && Time.time - lastDamageTime >= damageCooldown)
        {
            // attempt to get CharacterStats on the player and deal damage
            CharacterStats stats = player.GetComponent<CharacterStats>();
            if (stats != null)
            {
                stats.TakeDamage(damageAmount);
                lastDamageTime = Time.time;
                // optional: trigger attack animation
                if (anim != null) anim.SetTrigger("Attack");
            }
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null) agent.isStopped = true;
        gameObject.SetActive(false);

        Mask.SetActive(false);
        MasksShattered.SetActive(true);
    }
}