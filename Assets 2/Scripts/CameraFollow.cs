using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public CharacterController controller;  // drag the player’s CharacterController here
    public float normalHeight = 2.0f;       // set to your controller’s normal height
    public float heightTolerance = 0.05f;   // small tolerance
    public float followLerp = 12f;

    Vector3 offset;

    void Start()
    {
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        Vector3 target = player.position + offset;

        target.x = transform.position.x;

        bool sliding = controller != null && controller.height < (normalHeight - heightTolerance);
        if (sliding)
            target.y = transform.position.y;

        transform.position = Vector3.Lerp(transform.position, target, followLerp * Time.deltaTime);
    }
}
