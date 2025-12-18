using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//once the burger is complete, the player can click the plate and it will read if the correct order has been made.
//if the order is correct or incorrect, then that will be displayed on the screen

public class ServePlate : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public float displayTime = 2f;

    void Start()
    {
        if (resultText != null)
            resultText.text = "";
    }

    void Update()
    {

    }

    private void OnMouseDown()
    {
        if (GameFlow.orderValue == GameFlow.plateValue)
        {
            StartCoroutine(ShowResult("CORRECT!", Color.green));
        }
        else
        {
            StartCoroutine(ShowResult("INCORRECT!", Color.red));
        }
    }

    private IEnumerator ShowResult(string message, Color color)
    {
        if (resultText != null)
        {
            resultText.text = message;
            resultText.color = color;
        }

        yield return new WaitForSeconds(displayTime);

    }
}