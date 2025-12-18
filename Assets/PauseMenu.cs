using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    [SerializeField] private AudioClip buttonPressClip; 

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

    public void Resume()
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

    public void LoadSettings()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, Camera.main.transform, 1f); 

        SettingsScript.settingsPrevScene = 1;
        SceneManager.LoadScene("Settings Menu");

        //connect these functions to the buttons

    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

}
