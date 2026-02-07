using UnityEngine;

public class EnemyHitboxEventProxy : MonoBehaviour
{
    [Tooltip("Reference to the EnemyHitbox (can be on a child/hand)")]
    public EnemyHitbox hitbox;

    // Called from Animation Event to perform the hit overlap check
    public void DealDamage()
    {
        if (hitbox == null)
        {
            Debug.LogWarning("EnemyHitboxEventProxy: hitbox not assigned.");
            return;
        }
        hitbox.DealDamage();
    }

    // Optional helpers to enable/disable a collider-based hitbox via animation events
    public void EnableHitbox()
    {
        if (hitbox == null) return;
        var col = hitbox.GetComponent<Collider>();
        if (col != null) col.enabled = true;
    }

    public void DisableHitbox()
    {
        if (hitbox == null) return;
        var col = hitbox.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}