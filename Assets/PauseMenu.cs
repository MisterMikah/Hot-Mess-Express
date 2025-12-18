using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) //game is paused
            {
                Resume();
            }
            else //game is not paused
            {
                Pause();
            }
        }
    }

    private void Resume()
    {
        Time.timeScale = 1f; //resume time in scene
        SoundFXManager.instance.ResumeClip(); //resume audio clip

        pauseMenuUI.SetActive(false); //makes pause menu disappear
        isPaused = false;
    }

    private void Pause()
    {
        Time.timeScale = 0f; //freeze time in scene
        SoundFXManager.instance.PauseClip(); //pause audio clip

        pauseMenuUI.SetActive(true); //makes pause menu appear
        isPaused = true;
    }
}
