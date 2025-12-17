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
    public AudioClip runningClip;

    private CharacterController cc;
    private PlayerMovement movement;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement>();

        if (maxHearts < 1) maxHearts = 1;
        currentHearts = maxHearts;

        if (healthUI == null)
            healthUI = FindObjectOfType<PlayerHealthUI>();

        if (healthUI != null)
            healthUI.SetHearts(currentHearts, maxHearts);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (currentHearts <= 0) return;

        // All hittable stuff should be tagged "Obstacle" (trucks, fences, dividers)
        if (!hit.collider.CompareTag("Obstacle")) return;

        bool headOn = IsHeadOn(hit);

        if (headOn)
        {
            // straight into something
            if (SoundFXManager.instance != null && frontHitClip != null)
            {
                SoundFXManager.instance.DestroyLoopingFXClip();
                SoundFXManager.instance.PlaySoundFXClip(frontHitClip, transform, 1f);
            }

            Die();
        }
        else
        {
            HandleSideHit();
        }
    }

    void HandleSideHit()
    {
        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.DestroyLoopingFXClip();

            if (sideHitClip != null)
                SoundFXManager.instance.PlaySoundFXClip(sideHitClip, transform, 1f);

            if (runningClip != null)
                SoundFXManager.instance.PlayLoopingFXClip(runningClip, transform, 1f);
        }

        TakeDamage(1);

        if (movement != null)
            movement.RevertLane();
    }

    // Decide if this is a head-on hit or a side bump based on HOW we were moving
    bool IsHeadOn(ControllerColliderHit hit)
    {
        Vector3 move = hit.moveDirection;
        move.y = 0f;

        if (move.sqrMagnitude < 0.0001f)
        {
            // weird degenerate case, just treat as side
            return false;
        }

        // local Z = forward, X = sideways
        Vector3 localMove = transform.InverseTransformDirection(move);
        float absX = Mathf.Abs(localMove.x);
        float absZ = Mathf.Abs(localMove.z);

        // Bias toward side hits a bit so "almost sideways" counts as side
        return absZ >= absX * 1.2f;
    }

    void TakeDamage(int amount)
    {
        if (currentHearts <= 0) return;

        currentHearts -= amount;
        if (currentHearts < 0) currentHearts = 0;

        if (healthUI != null)
            healthUI.SetHearts(currentHearts, maxHearts);

        if (currentHearts <= 0)
        {
            if (SoundFXManager.instance != null)
                SoundFXManager.instance.DestroyLoopingFXClip();

            Die();
        }
    }

    void Die()
    {
        if (Time.timeScale == 0f) return; // already dead
        Time.timeScale = 0f;
        Debug.Log("GAME OVER");
        // TODO: show Game Over UI
    }
}
