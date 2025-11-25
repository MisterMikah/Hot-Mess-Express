using UnityEngine;

public class TruckMover : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Move in the truck's forward direction.
        // Since we spawned them with Y = 180, this sends them toward the player.
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }
}
