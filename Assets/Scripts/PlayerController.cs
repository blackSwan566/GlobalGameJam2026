using UnityEngine;

public class PlayerController : MonoBehaviour
{
   // [Header("Bewegung")]
    public float speed = 5f;
    public CharacterController controller;

   // [Header("Maus-Steuerung")]
    public float mouseSensitivity = 100f;

    void Start()
    {
        // Sperrt die Maus in der Mitte, damit sie beim Drehen nicht aus dem Fenster rutscht
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. MAUS-ROTATION (Nur Links/Rechts)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // Dreht das ganze GameObject um die eigene Achse
        transform.Rotate(Vector3.up * mouseX);


        // 2. BEWEGUNG
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Die Bewegung erfolgt jetzt immer relativ zur Blickrichtung des Players
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
        
        // 3. SCHIEẞEN
        if (Input.GetButtonDown("Fire1"))
        {
            // Hier bleibt dein bestehender Shoot-Code
        }
    }
}