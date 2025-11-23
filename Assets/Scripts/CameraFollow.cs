using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public PlayerMovement playerMovement;   // drag your player here
    public float followLerp = 12f;          // smoothness (optional)

    Vector3 offset;

    void Start()
    {
        offset = transform.position - player.position;
    }

    void Update()
    {
        Vector3 target = player.position + offset;

        // Keep camera centered horizontally (don’t track lane X)
        target.x = transform.position.x;

        // If sliding, freeze Y; if not, follow Y (so jumps are tracked)
        if (playerMovement != null && playerMovement.IsSliding)
            target.y = transform.position.y;

        // Smooth move (optional)
        transform.position = Vector3.Lerp(transform.position, target, followLerp * Time.deltaTime);
    }
}
