using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverPanel;   // panel with retry / menu buttons
    public TMP_Text messageText;       // optional (can be null)
    public Button retryButton;
    public Button menuButton;

    [Header("Scenes")]
    public string gameSceneName = "RunnerScene"; // set in Inspector
    public string mainMenuSceneName = "MainMenu"; // set in Inspector

    private Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        else
            Debug.LogWarning("GameOverUI: gameOverPanel is NOT assigned.");

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetry);
        else
            Debug.LogWarning("GameOverUI: retryButton is NOT assigned.");

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenu);
        else
            Debug.LogWarning("GameOverUI: menuButton is NOT assigned.");
    }

    public void ShowGameOver()
    {
        // make sure canvas is on
        if (parentCanvas != null && !parentCanvas.gameObject.activeSelf)
            parentCanvas.gameObject.SetActive(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (messageText != null)
            messageText.text = "THE FOOD WAS DESTROYED";
    }

    private void OnRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
