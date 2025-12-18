using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [Header("Health")]
    public int maxHearts = 3;
    public int currentHearts;

    [Header("UI")]
    public PlayerHealthUI healthUI;   // burger hearts
    private GameOverUI gameOverUI;    // new

    [Header("Audio")]
    public AudioClip frontHitClip;
    public AudioClip sideHitClip;
    public AudioClip runningClip;

    private CharacterController cc;
    private PlayerMovement movement;

    private bool collisionsEnabled = true;
    private bool isDead = false;

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

        // find GameOverUI even if its canvas starts disabled
        gameOverUI = FindObjectOfType<GameOverUI>(true);
        if (gameOverUI == null)
        {
            Debug.LogWarning("PlayerCollision: No GameOverUI found in scene.");
        }
    }

    public void DisableCollisions()
    {
        collisionsEnabled = false;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!collisionsEnabled || isDead) return;
        if (currentHearts <= 0) return;

        if (!hit.collider.CompareTag("Obstacle")) return;

        bool headOn = IsHeadOn(hit);

        if (headOn)
        {
            // straight into something: instant game over
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

    bool IsHeadOn(ControllerColliderHit hit)
    {
        Vector3 move = hit.moveDirection;
        move.y = 0f;

        if (move.sqrMagnitude < 0.0001f)
            return false; // treat weird case as side

        Vector3 localMove = transform.InverseTransformDirection(move);
        float absX = Mathf.Abs(localMove.x);
        float absZ = Mathf.Abs(localMove.z);

        // forward-dominant = head-on
        return absZ >= absX * 1.2f;
    }

    void TakeDamage(int amount)
    {
        if (isDead) return;
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
        if (isDead) return;
        isDead = true;
        collisionsEnabled = false;

        Debug.Log("PlayerCollision: GAME OVER");

        // freeze gameplay
        Time.timeScale = 0f;

        // show game-over UI
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("PlayerCollision: gameOverUI is null, cannot show Game Over UI.");
        }
    }
}
