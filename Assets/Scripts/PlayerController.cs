using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public CharacterController controller;
    public GameObject bulletPrefab;
    public Transform firePoint;


    public float speed = 8f;
    public float gravity = -15f;
    public float mouseSensitivity = 2f;

    private Vector3 velocity;

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();

        // Maus einsperren
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 2. AWDS BEWEGUNG
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Wichtig: transform.forward bezieht sich jetzt auf die neue Drehung
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // 3. SCHWERKRAFT
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        velocity.y = Mathf.Clamp(velocity.y, -20f, 10f);
        controller.Move(velocity * Time.deltaTime);

        // 4. SCHIESSEN
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (bulletPrefab != null && firePoint != null)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            }
        }
    }
}