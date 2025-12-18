using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    public string endSceneName = "EndScene";  // <-- set this in Inspector
    public float danceDuration = 5f;

    private bool finished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;
        if (!other.CompareTag("Player")) return;

        finished = true;
        Debug.Log("FinishTrigger: Player reached finish.");

        // Get required components
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        PlayerCollision collision = other.GetComponent<PlayerCollision>();
        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();

        // ⭐ SAVE HEART DATA FOR END SCENE ⭐
        if (collision != null)
        {
            RunResult.heartsLeft = collision.currentHearts;
            RunResult.maxHearts = collision.maxHearts;
        }

        // Stop obstacles
        if (spawner != null)
            spawner.StopSpawning();

        // Disable collision deaths
        if (collision != null)
            collision.DisableCollisions();

        // Stop and rotate and dance
        if (movement != null)
            movement.EndRun();

        // Load next scene after delay
        StartCoroutine(LoadEndSceneAfterDelay());
    }

    private System.Collections.IEnumerator LoadEndSceneAfterDelay()
    {
        Debug.Log("FinishTrigger: Waiting to load end scene...");
        yield return new WaitForSeconds(danceDuration);

        Debug.Log("FinishTrigger: Loading scene '" + endSceneName + "'");
        SceneManager.LoadScene(endSceneName);
    }
}
