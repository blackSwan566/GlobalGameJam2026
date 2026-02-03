using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float backSpeedMultiplier = 0.5f; // backing is moveSpeed * this
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 10f; // higher = snappier rotation

    [Header("References")]
    [SerializeField] private Transform cameraTransform; // optional, for camera-relative movement
    [SerializeField] private Animator animator; // optional, assign your Animator

    [Header("Animator Parameters")]
    [SerializeField] private string speedParam = "Speed";       // float 0..1 for blend tree
    [SerializeField] private string isMovingParam = "IsMoving"; // bool
    [SerializeField] private string isBackingParam = "IsBacking"; // bool for backward animation

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;

    // public states for animation/other systems
    public bool IsMoving { get; private set; }
    public bool IsBacking { get; private set; }
    public Vector3 Velocity => velocity;

    // animator hashes for performance
    private int speedHash;
    private int isMovingHash;
    private int isBackingHash;

    // Allows external scripts to temporarily disable movement (e.g. during attack)
    public bool MovementEnabled { get; set; } = true;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (animator == null)
            animator = GetComponent<Animator>();

        speedHash = Animator.StringToHash(speedParam);
        isMovingHash = Animator.StringToHash(isMovingParam);
        isBackingHash = Animator.StringToHash(isBackingParam);
    }

    void Update()
    {
        if (MovementEnabled)
            HandleMovement();
        else
            // still apply gravity if movement disabled
            ApplyGravityMovementOnly();

        UpdateAnimator();
    }

    private void HandleMovement()
    {
        // read raw inputs
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // compute camera-relative movement (ignore camera pitch)
        Vector3 forward = (cameraTransform != null) ? Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized : Vector3.forward;
        Vector3 right = (cameraTransform != null) ? cameraTransform.right : Vector3.right;

        Vector3 desiredMove = right * moveInput.x + forward * moveInput.y;
        float inputMagnitude = desiredMove.magnitude;

        IsMoving = inputMagnitude > 0.01f;

        // Determine backing (pressing S / negative vertical input relative to camera-forward)
        IsBacking = Input.GetAxisRaw("Vertical") < -0.1f && Mathf.Abs(Input.GetAxisRaw("Vertical")) > Mathf.Abs(Input.GetAxisRaw("Horizontal"));

        // Rotation:
        if (IsMoving && !IsBacking)
        {
            Vector3 flatMove = desiredMove;
            flatMove.y = 0f;
            if (flatMove.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(flatMove.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        // Movement speed: use backing multiplier when backing
        float effectiveSpeed = IsBacking ? moveSpeed * backSpeedMultiplier : moveSpeed;

        Vector3 horizontalVelocity;
        if (IsMoving)
        {
            horizontalVelocity = desiredMove.normalized * effectiveSpeed;
        }
        else
        {
            horizontalVelocity = Vector3.zero;
        }

        // preserve vertical velocity (gravity)
        Vector3 move = horizontalVelocity + Vector3.up * velocity.y;
        controller.Move(move * Time.deltaTime);

        // store horizontal velocity for external use
        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;

        // gravity accumulation handled below
        ApplyGravity();
    }

    // Apply gravity but don't move horizontally (used when movement is disabled)
    private void ApplyGravityMovementOnly()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(Vector3.up * velocity.y * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        float horizontalSpeed = new Vector2(velocity.x, velocity.z).magnitude;
        float normalized = (moveSpeed > 0f) ? Mathf.Clamp01(horizontalSpeed / moveSpeed) : 0f;

        animator.SetFloat(speedHash, normalized);
        animator.SetBool(isMovingHash, IsMoving);
        animator.SetBool(isBackingHash, IsBacking);
    }
}