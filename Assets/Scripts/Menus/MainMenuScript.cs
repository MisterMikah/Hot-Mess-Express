using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    //audio
    [SerializeField] private AudioClip buttonPressClip; 
    [SerializeField] private AudioClip music;


    /*****************************************************************
    
    //INCLUDE ON AWAKE FUNCTION THAT PLAYS MUSIC WHEN GAME STARTS UP
    public void Awake()
    {
        if (music != null)
        {
            MusicManager.instance.OnPlayMusic(music, Camera.main.transform, 1f);
        }
        else
        {
            Debug.Log("main menu does not have music assigned");
        }
        
    }

    *****************************************************************/

    public void PlayGame() //starts game when press start button
    {
        //play bell ring sound
        SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, Camera.main.transform, 1f); 

        SceneManager.LoadScene("HotMess"); //Scene_1, HotMess
        //REMINDERS:
        //      - change string name from "Scene_1" to whatever the real scene will 
        //          officially be called
        //      - in build settings make sure to add the scenes that it will navigate to 
        //             - how to get to build settings: file -> build settings
    }

    public void LoadSettingsMenu() //loads settings menu when press settings button
    {
        //play button press sound
        SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, Camera.main.transform, 1f); 

        SettingsScript.settingsPrevScene = 0;
        SceneManager.LoadScene("Settings Menu");
    }

    public void QuitGame() //quits game when press exit button
    {
        //play button press sound
        SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, Camera.main.transform, 1f); 

        Debug.Log("Exit");
        Application.Quit();
    }
}
