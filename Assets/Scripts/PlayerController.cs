using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 8f;
    public float gravity = -9.81f;

    private Vector3 velocity;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. WASD INPUT
        float x = Input.GetAxis("Horizontal"); // A und D
        float z = Input.GetAxis("Vertical");   // W und S

        // Richtung berechnen (relativ zur Blickrichtung des Spielers)
        Vector3 move = transform.right * x + transform.forward * z;

        // Laufen ausführen
        controller.Move(move * speed * Time.deltaTime);

        // 2. STABILER BODEN-CHECK
        // Wenn wir am Boden sind, lassen wir die Schwerkraft nicht ins Unendliche wachsen
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Drückt uns sanft auf den Boden
        }
        else
        {
            // Wenn wir in der Luft sind (z.B. über einer Senke), fallen wir normal
            velocity.y += gravity * Time.deltaTime;
        }

        // 3. SCHWERKRAFT AUSFÜHREN
        controller.Move(velocity * Time.deltaTime);
    }
}