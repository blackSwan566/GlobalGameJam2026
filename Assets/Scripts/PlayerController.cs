using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Animator anim;
    public float maxSpeed = 5f;
    public float gravity = -9.81f;

    private Vector3 velocity;

    void Update()
    {
        // 1. Input holen
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        // 2. Bewegen
        controller.Move(move * maxSpeed * Time.deltaTime);

        // 3. Schwerkraft (damit er nicht schwebt)
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 4. Animationen
        anim.SetFloat("Speed", move.magnitude);

        if (Input.GetMouseButtonDown(0)) anim.SetTrigger("Attack");
    }
}