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

        SettingsScript.settingsPrevScene = 2;

        //save prev scene

        //gonna test settings menu where instead of being a separate scene, it is a layer of ui

        SceneManager.LoadScene("Settings Menu");

    }

    public void QuitToMenu()
    {
        //destroy audio clips from current scene
        SoundFXManager.instance.DestroyLoopingFXClip();

        //play button click sound
        SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, Camera.main.transform, 1f);

        SceneManager.LoadScene("Main Menu");
    }

}
