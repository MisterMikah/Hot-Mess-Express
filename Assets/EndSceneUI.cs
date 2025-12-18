using UnityEngine;
using TMPro;

public class EndSceneUI : MonoBehaviour
{
    public TMP_Text foodHealthText;

    void Start()
    {
        int heartsLeft = RunResult.heartsLeft;
        int maxHearts = RunResult.maxHearts;

        if (maxHearts <= 0)
        {
            foodHealthText.text = "Food Health: 100%";
            foodHealthText.color = Color.green;
            return;
        }

        float percent = (float)heartsLeft / maxHearts * 100f;
        int rounded = Mathf.RoundToInt(percent);

        // SPECIAL CONGRATS MESSAGE
        if (heartsLeft == maxHearts)
        {
            foodHealthText.text = $"CONGRATS! Food Health: {rounded}%";
            foodHealthText.color = Color.green;
            return;
        }

        // NORMAL MESSAGE
        foodHealthText.text = $"Food Health: {rounded}%";

        // COLOR LOGIC
        if (rounded >= 67)
        {
            // 66% – 100%
            foodHealthText.color = Color.yellow;
        }
        else if (rounded >= 34)
        {
            // 33% – 66%
            foodHealthText.color = new Color(1f, 0.5f, 0f); // orange-ish
        }
        else
        {
            // 0% – 33%
            foodHealthText.color = Color.red;
        }
    }
}
