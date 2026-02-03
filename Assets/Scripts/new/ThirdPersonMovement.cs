using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;

    // 👉 Öffentliche Zustände für Animation
    public bool IsMoving { get; private set; }
    public Vector3 Velocity => velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        // Lokale Bewegung (vorwärts = Z)
        move = transform.TransformDirection(move);

        controller.Move(move * moveSpeed * Time.deltaTime);

        IsMoving = moveInput.sqrMagnitude > 0.01f;
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f; // hält den Controller am Boden

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
