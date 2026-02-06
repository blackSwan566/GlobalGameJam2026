using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyHitbox : MonoBehaviour
{
    [Tooltip("Amount of damage to apply to the player on hit")]
    public int damage = 1;

    [Tooltip("Minimum time between hits from this hitbox (seconds) to avoid multi-hits)")]
    public float hitCooldown = 0.4f;

    [Header("Overlap (used by animation-event hits)")]
    [Tooltip("If using animation events, this radius defines how large the hit area is around this transform")]
    public float overlapRadius = 0.3f;
    [Tooltip("Layers to hit (set to player's layer)")]
    public LayerMask playerLayer;

    [Header("Optional: require enemy to be attacking")]
    [Tooltip("If true, the hit only applies when attackStateProvider has a true bool named attackStateBoolName")]
    public bool requireAttackState = false;
    public MonoBehaviour attackStateProvider; // a script that has a public bool or field IsAttacking
    public string attackStateBoolName = "IsAttacking";

    [Header("Debug")]
    public bool debugLogs = true;
    public bool drawGizmo = true;
    public Color gizmoColor = Color.red;

    float lastHitTime = -999f;

    void Reset()
    {
        // recommended: make this collider a trigger (but not required if using overlap method)
        var c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;
    }

    // Fallbacks for direct physics collisions / triggers
    private void OnTriggerEnter(Collider other)
    {
        if (debugLogs) Debug.Log($"[EnemyHitbox] OnTriggerEnter with {other.name} tag={other.tag}");
        TryHitCollider(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (debugLogs) Debug.Log($"[EnemyHitbox] OnCollisionEnter with {collision.collider.name} tag={collision.collider.tag}");
        TryHitCollider(collision.collider);
    }

    // Try to apply a single hit from a collider callback (best-effort)
    void TryHitCollider(Collider other)
    {
        if (other == null) return;

        // Optional attack-state requirement
        if (requireAttackState && attackStateProvider != null)
        {
            if (!ProviderIsAttacking()) 
            {
                if (debugLogs) Debug.Log("[EnemyHitbox] Ignored hit: provider not attacking.");
                return;
            }
        }

        if (Time.time - lastHitTime < hitCooldown)
        {
            if (debugLogs) Debug.Log("[EnemyHitbox] Ignored hit: cooldown");
            return;
        }

        // Attempt to find PlayerHealth on collided object / parents
        PlayerHealth ph = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>() ?? other.GetComponentInChildren<PlayerHealth>();
        if (ph != null)
        {
            if (debugLogs) Debug.Log($"[EnemyHitbox] Collided with player '{ph.gameObject.name}', applying {damage} damage.");
            ph.TakeDamage(damage);
            lastHitTime = Time.time;
        }
        else
        {
            if (debugLogs) Debug.LogWarning($"[EnemyHitbox] Collided object has no PlayerHealth: {other.name} (root: {other.transform.root.name}).");
        }
    }

    // Recommended method: call this from an animation event at the exact hit frame
    // Example: add an Animation Event on the attack clip that calls "DealDamage" (no args).
    public void DealDamage()
    {
        if (requireAttackState && attackStateProvider != null && !ProviderIsAttacking())
        {
            if (debugLogs) Debug.Log("[EnemyHitbox] DealDamage ignored: provider not attacking.");
            return;
        }

        if (Time.time - lastHitTime < hitCooldown)
        {
            if (debugLogs) Debug.Log("[EnemyHitbox] DealDamage ignored: cooldown");
            return;
        }

        // Overlap check
        Collider[] hits = Physics.OverlapSphere(transform.position, overlapRadius, playerLayer, QueryTriggerInteraction.Collide);

        if (hits.Length == 0)
        {
            if (debugLogs) Debug.Log("[EnemyHitbox] DealDamage: no players in overlap.");
            lastHitTime = Time.time; // still set cooldown to avoid repeated calls in same frame if desired
            return;
        }

        bool anyHit = false;
        foreach (var col in hits)
        {
            PlayerHealth ph = col.GetComponent<PlayerHealth>() ?? col.GetComponentInParent<PlayerHealth>() ?? col.GetComponentInChildren<PlayerHealth>();
            if (ph != null)
            {
                if (debugLogs) Debug.Log($"[EnemyHitbox] DealDamage: hit player '{ph.gameObject.name}' with collider '{col.name}'");
                ph.TakeDamage(damage);
                anyHit = true;
            }
        }

        if (anyHit) lastHitTime = Time.time;
    }

    bool ProviderIsAttacking()
    {
        if (attackStateProvider == null) return false;
        var t = attackStateProvider.GetType();
        var prop = t.GetProperty(attackStateBoolName);
        if (prop != null && prop.PropertyType == typeof(bool))
            return (bool)prop.GetValue(attackStateProvider);
        var field = t.GetField(attackStateBoolName);
        if (field != null && field.FieldType == typeof(bool))
            return (bool)field.GetValue(attackStateProvider);

        // If no field/property found, assume true (or change behavior if you want)
        if (debugLogs) Debug.LogWarning($"[EnemyHitbox] attackStateProvider has no bool '{attackStateBoolName}'.");
        return true;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmo) return;
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }
}