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

        if (SoundFXManager.instance.audioSource != null)
        {
            SoundFXManager.instance.ResumeClip(); //resume current audio clip if it exists
        }
        
        pauseMenuUI.SetActive(false); //makes pause menu disappear
        isPaused = false;
    }

    private void Pause()
    {
        Time.timeScale = 0f; //freeze time in scene

        if (SoundFXManager.instance.audioSource != null)
        {
            SoundFXManager.instance.PauseClip(); //pause current audio clip if it exists
        }
    
        pauseMenuUI.SetActive(true); //makes pause menu appear
        isPaused = true;
    }

    public void LoadSettings()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, Camera.main.transform, 1f); 

        
        if (SceneManager.GetActiveScene().buildIndex == 3) //paused from cooking minigame
        {
            SettingsScript.settingsPrevScene = 1;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2) //paused from runner
        {
            SettingsScript.settingsPrevScene = 2;
        }

        SceneManager.LoadScene("Settings Menu");

    }

    public void QuitToMenu()
    {
        //destroy audio clips from current scene if they exist
        if (SoundFXManager.instance.audioSource != null)
        {
            SoundFXManager.instance.DestroyLoopingFXClip();
        }

        //play button click sound
        SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, Camera.main.transform, 1f);

        SceneManager.LoadScene("Main Menu");
    }

}
