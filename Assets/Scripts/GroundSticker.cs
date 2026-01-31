using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GroundSticker : MonoBehaviour
{
    public float downForce = 50f;     // Wie stark er klebt
    public float rayLength = 1.5f;    // Länge des Checks (bisschen mehr als halbe Player-Höhe)
    public LayerMask groundLayer;     // Layer deines Custom Grounds

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void FixedUpdate()
    {
        // Schießt Strahl nach unten
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayLength, groundLayer))
        {
            // Drückt den Player exakt senkrecht zur Bodenoberfläche (Normale) nach unten
            rb.AddForce(-hit.normal * downForce, ForceMode.Acceleration);
        }
    }
}