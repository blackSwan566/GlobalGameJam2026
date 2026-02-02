using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // leave empty to use Camera.main at Start

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public bool enableSprint = true;

    [Header("Rotation & Smoothing")]
    public float rotationSmoothTime = 0.12f;
    public float speedSmoothTime = 0.1f;

    [Header("Jump / Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    // private state
    CharacterController controller;
    float currentSpeed;
    float speedVelocity;
    float rotationVelocity;
    float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Read raw input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Disable backward movement (S): clamp vertical to >= 0
        input.y = Mathf.Max(0f, input.y);

        float inputMagnitude = Mathf.Clamp01(input.magnitude);

        // Determine target speed (sprint if holding Left Shift and enabled)
        bool sprint = enableSprint && Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = (sprint ? runSpeed : walkSpeed) * inputMagnitude;

        // Smooth speed
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, speedSmoothTime);

        // Calculate movement direction relative to camera (or world if camera missing)
        Vector3 moveDir;
        if (inputMagnitude > 0.01f)
        {
            // angle based on input and camera yaw
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            float cameraYaw = (cameraTransform != null) ? cameraTransform.eulerAngles.y : 0f;
            float desiredAngle = targetAngle + cameraYaw;

            // Smooth rotation
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, desiredAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move forward in the rotated direction
            moveDir = Quaternion.Euler(0f, desiredAngle, 0f) * Vector3.forward;
        }
        else
        {
            moveDir = Vector3.zero;
        }

        // Grounded handling & gravity
        if (controller.isGrounded)
        {
            // small negative to keep controller grounded
            if (verticalVelocity < 0f) verticalVelocity = -2f;

            // Jump
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;

        // Compose final velocity and move
        Vector3 velocity = moveDir.normalized * currentSpeed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}