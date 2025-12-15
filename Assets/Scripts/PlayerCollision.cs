using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [Header("Health")]
    public int maxHearts = 3;
    public int currentHearts;

    [Header("UI")]
    public PlayerHealthUI healthUI;   // drag BurgerBar here

    [Header("Audio")]
    public AudioClip frontHitClip;
    public AudioClip sideHitClip;


    private CharacterController cc;
    private PlayerMovement movement;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement>();

        if (maxHearts < 1) maxHearts = 1;
        currentHearts = maxHearts;

        if (healthUI == null)
        {
            healthUI = FindObjectOfType<PlayerHealthUI>();
        }

        if (healthUI != null)
            healthUI.SetHearts(currentHearts, maxHearts);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (currentHearts <= 0) return;
        if (!hit.collider.CompareTag("Obstacle")) return;

        bool headOn = IsHeadOn(hit);

        if (headOn)
        {
            // straight into something in your lane

            //play front/straight hit collision sound
            if (SoundFXManager.instance != null && frontHitClip != null)
            {
                SoundFXManager.instance.PlaySoundFXClip(frontHitClip, transform, 1f);
            }

            Die();
        }
        else
        {
            // play side hit collision sound 
            if (SoundFXManager.instance != null && sideHitClip != null)
            {
                SoundFXManager.instance.PlaySoundFXClip(sideHitClip, transform, 1f);
            }

            // side swipe: lose burger + snap back
            TakeDamage(1);
            if (movement != null)
                movement.RevertLane();
        }
    }

    // Decide if this is a head-on hit or a side bump based on lane alignment
    bool IsHeadOn(ControllerColliderHit hit)
    {
        if (movement == null) return true; // be safe

        // center X of obstacle vs player
        float playerX = transform.position.x;
        float obstacleX = hit.collider.bounds.center.x;

        // if almost same lane center in X → treat as head-on
        float laneWidth = movement.laneWidth;
        float dx = Mathf.Abs(obstacleX - playerX);

        // tune the 0.4f factor: smaller = stricter front hits only
        return dx < laneWidth * 0.4f;
    }

    void TakeDamage(int amount)
    {
        if (currentHearts <= 0) return;

        currentHearts -= amount;
        if (currentHearts < 0) currentHearts = 0;

        if (healthUI != null)
            healthUI.SetHearts(currentHearts, maxHearts);

        if (currentHearts <= 0)
            Die();
    }

    void Die()
    {
        if (Time.timeScale == 0f) return; // already dead
        Time.timeScale = 0f;
        Debug.Log("GAME OVER");
        // TODO: show Game Over UI
    }
}
