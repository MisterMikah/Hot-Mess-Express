using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ServePlate : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public float displayTime = 2f;

    [Header("Scenes")]
    public string runnerSceneName = "RunnerScene";  // set this in Inspector

    void Start()
    {
        if (resultText != null)
            resultText.text = "";
    }

    private void OnMouseDown()
    {
        // correct
        if (GameFlow.orderValue == GameFlow.plateValue)
        {
            StartCoroutine(ShowResultAndContinue("CORRECT!", Color.green, true));
        }
        else // incorrect
        {
            StartCoroutine(ShowResultAndContinue("INCORRECT!", Color.red, false));
        }
    }

    private IEnumerator ShowResultAndContinue(string message, Color color, bool correct)
    {
        if (resultText != null)
        {
            resultText.text = message;
            resultText.color = color;
        }

        yield return new WaitForSeconds(displayTime);

        if (correct)
        {
            // Load next level → the runner scene
            SceneManager.LoadScene(runnerSceneName);
        }
        else
        {
            // Reload current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
