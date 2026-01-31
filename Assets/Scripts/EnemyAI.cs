using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Komponenten")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;

    [Header("Anti-Stuck & Performance")]
    public float stuckThreshold = 0.5f;
    private Vector3 lastPosition;
    private float stuckTimer;
    private float pathUpdateTimer;
    private float updateInterval = 0.5f; // Pfad wird alle 0.5s aktualisiert

    void Start()
    {
        // Holt sich die Komponenten automatisch, falls nicht im Inspector zugewiesen
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();

        lastPosition = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        // 1. PFAD-UPDATE (Regelmäßiger "Reset", damit sie nicht stehen bleiben)
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= updateInterval)
        {
            pathUpdateTimer = 0;
            UpdatePath();
        }

        // 2. ANIMATION (Basierend auf tatsächlicher Bewegung)
        if (anim != null)
        {
            float currentSpeed = agent.velocity.magnitude;
            anim.SetFloat("Speed", currentSpeed);
        }

        // 3. STUCK-DETECTION (Befreiung bei hängendem Mesh)
        CheckIfStuck();
    }

    void UpdatePath()
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            // Rettung: Falls der Gegner vom NavMesh gerutscht ist (Hügel-Glitch)
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    void CheckIfStuck()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        // Wenn er sich trotz Pfad nicht bewegt...
        if (distanceMoved < 0.05f && agent.hasPath)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0;
        }

        // Wenn er zu lange feststeckt -> Zufälliger Ausbruch
        if (stuckTimer > stuckThreshold)
        {
            TryEscape();
            stuckTimer = 0;
        }

        lastPosition = transform.position;
    }

    void TryEscape()
    {
        // Sucht einen Punkt in 2m Umkreis zum "Freischwimmen"
        Vector3 randomDirection = Random.insideUnitSphere * 2f;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        // Minimaler Hopser nach oben gegen Mesh-Clipping
        transform.position += Vector3.up * 0.02f;
    }
}