using UnityEngine;

public class TruckMover : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Move along the truck's forward direction.
        // Since we rotated it 180° on Y, this will move toward the player.
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }
}
