using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private ThirdPersonMovement movementController; // optional, used to block movement during attack

    [Header("Animator Parameters")]
    [SerializeField] private string attackTriggerParam = "Attack";     // trigger in animator
    [SerializeField] private string isAttackingParam = "IsAttacking";  // optional bool param to reflect state

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.4f;   // minimal time between attacks
    [SerializeField] private float attackDuration = 0.6f;   // only used if not using animation event
    [SerializeField] private bool useAnimationEventToEndAttack = true;
    [SerializeField] private bool blockMovementDuringAttack = true; // if true, disables movementController.MovementEnabled during attack

    [Header("Events (optional)")]
    public UnityEvent OnAttackStarted;
    public UnityEvent OnAttackEnded;

    private int attackTriggerHash;
    private int isAttackingHash;

    private bool isAttacking = false;
    private float cooldownTimer = 0f;
    private float attackTimer = 0f;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        attackTriggerHash = Animator.StringToHash(attackTriggerParam);
        isAttackingHash = Animator.StringToHash(isAttackingParam);
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (!isAttacking && Input.GetButtonDown("Fire1") && cooldownTimer <= 0f)
        {
            StartAttack();
        }

        if (!useAnimationEventToEndAttack && isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
                EndAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        cooldownTimer = attackCooldown;

        // Trigger animator
        if (animator != null)
        {
            animator.SetTrigger(attackTriggerHash);
            if (!string.IsNullOrEmpty(isAttackingParam))
                animator.SetBool(isAttackingHash, true);
        }

        // Optionally block movement
        if (blockMovementDuringAttack && movementController != null)
            movementController.MovementEnabled = false;

        if (!useAnimationEventToEndAttack)
            attackTimer = attackDuration;

        OnAttackStarted?.Invoke();
    }

    // Call this from an Animation Event at the end of the attack animation, OR it will be invoked after attackDuration
    public void OnAttackEnd()
    {
        EndAttack();
    }

    private void EndAttack()
    {
        if (!isAttacking) return;

        isAttacking = false;

        if (animator != null && !string.IsNullOrEmpty(isAttackingParam))
            animator.SetBool(isAttackingHash, false);

        // Re-enable movement if it was blocked
        if (blockMovementDuringAttack && movementController != null)
            movementController.MovementEnabled = true;

        OnAttackEnded?.Invoke();
    }

    // Optional helper for other scripts:
    public bool IsAttacking() => isAttacking;
}