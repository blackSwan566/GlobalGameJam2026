using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Mouse")]
    public float mouseSensitivity = 3f;
    public float minY = -30f;
    public float maxY = 60f;

    [Header("Distance")]
    public float distance = 5f;
    public float minDistance = 0.5f;

    [Header("Collision")]
    public float collisionRadius = 0.25f;
    public float collisionOffset = 0.12f;     // keep camera off the surface
    public float collisionPadding = 0.05f;    // extra buffer to avoid near-plane clipping
    public LayerMask collisionLayers = ~0;

    [Header("Smoothing")]
    public float distanceSmoothSpeedOpen = 10f;  // speed when moving back out
    public float distanceSmoothSpeedClose = 80f; // much faster when moving in (prevents clipping)

    private float yaw;
    private float pitch;
    private float currentDistance;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentDistance = distance;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Mouse rotation
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 pivot = target.position + targetOffset;

        // desired camera position before collision handling
        Vector3 desiredOffset = rotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPos = pivot + desiredOffset;

        // direction from pivot to desired
        Vector3 dir = desiredPos - pivot;
        float desiredDist = dir.magnitude;
        Vector3 dirNormalized = desiredDist > 0.0001f ? dir / desiredDist : -(rotation * Vector3.forward);

        // SphereCast to detect obstacles between pivot and desired camera pos
        float adjustedDistance = distance;
        RaycastHit hit;
        if (Physics.SphereCast(pivot, collisionRadius, dirNormalized, out hit, desiredDist, collisionLayers, QueryTriggerInteraction.Ignore))
        {
            // keep some offset and padding to avoid near-plane clipping
            adjustedDistance = Mathf.Clamp(hit.distance - collisionOffset - collisionPadding, minDistance, distance);
        }

        // Move faster when closing in to avoid passing through geometry
        if (adjustedDistance < currentDistance)
        {
            currentDistance = Mathf.MoveTowards(currentDistance, adjustedDistance, Time.deltaTime * distanceSmoothSpeedClose);
        }
        else
        {
            currentDistance = Mathf.Lerp(currentDistance, adjustedDistance, Time.deltaTime * distanceSmoothSpeedOpen);
        }

        // Candidate final position
        Vector3 finalPos = pivot + rotation * new Vector3(0f, 0f, -currentDistance);

        // Extra safety: if the camera still overlaps geometry (e.g., due to smoothing or geometry misses),
        // compute the closest collision point and immediately correct the distance.
        Collider[] overlaps = Physics.OverlapSphere(finalPos, collisionRadius, collisionLayers, QueryTriggerInteraction.Ignore);
        if (overlaps.Length > 0)
        {
            float nearestDistFromPivot = float.MaxValue;
            foreach (var col in overlaps)
            {
                Vector3 closestOnCol = col.ClosestPoint(finalPos);
                float distFromPivot = Vector3.Distance(pivot, closestOnCol);
                if (distFromPivot < nearestDistFromPivot) nearestDistFromPivot = distFromPivot;
            }

            if (nearestDistFromPivot < float.MaxValue)
            {
                float nonOverlapDistance = Mathf.Clamp(nearestDistFromPivot - collisionOffset - collisionPadding, minDistance, distance);
                // snap in quickly to this non-overlapping distance
                currentDistance = Mathf.MoveTowards(currentDistance, nonOverlapDistance, Time.deltaTime * distanceSmoothSpeedClose);
                finalPos = pivot + rotation * new Vector3(0f, 0f, -currentDistance);
            }
        }

        // Apply final position and look
        transform.position = finalPos;
        transform.LookAt(pivot);
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.cyan;
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + targetOffset;
        Vector3 desiredPos = pivot + rot * new Vector3(0f, 0f, -distance);
        Gizmos.DrawWireSphere(desiredPos, collisionRadius);
        Gizmos.DrawLine(pivot, desiredPos);
    }
}