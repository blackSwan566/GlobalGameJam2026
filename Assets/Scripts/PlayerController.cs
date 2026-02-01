using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[Header("Bewegung & Physik")]
    public float speed = 8f;
    public float gravity = -15f;
    public float speed2;
    public CharacterController controller;

    //[Header("Schießen")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    // [Header("Maus-Steuerung")]
    public float mouseSensitivity = 100f;

    private Vector3 velocity;

    public Animator anim;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        // Sperrt die Maus in der Mitte und macht sie unsichtbar
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. MAUS-ROTATION (Nur Links/Rechts drehen des Körpers)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // 2. WASD BEWEGUNG
        float x = Input.GetAxis("Horizontal"); // A/D
         float z = Input.GetAxis("Vertical");   // W/S
     

        // Bewegung relativ zur Blickrichtung des Players
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // 3. SCHWERKRAFT (Damit der Player nicht schwebt)
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Kleiner Anpressdruck zum Boden
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Führt die Schwerkraft-Bewegung aus
        controller.Move(velocity * Time.deltaTime);

        speed2 = math.abs(x) + math.abs(z);

        anim.SetFloat("Run", speed2);
;
        // 4. SCHIEẞEN MIT SPACE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Erstellt die Kugel am Firepoint mit dessen Rotation
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}