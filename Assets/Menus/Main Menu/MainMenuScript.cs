using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public static Boolean prevSceneMain = false;

    [SerializeField] private AudioClip buttonPressClip; 

    public void PlayGame() //starts game when press start button
    {
        SceneManager.LoadScene("HotMess"); 
        //REMINDERS:
        //      - change string name from "Scene_1" to whatever the real scene will 
        //          officially be called
        //      - in build settings make sure to add the scenes that it will navigate to 
        //             - how to get to build settings: file -> build settings
    }

    public void LoadSettingsMenu() //loads settings menu when press settings button
    {
        prevSceneMain = true;
        SceneManager.LoadScene("Settings Menu");
    }

    public void QuitGame() //quits game when press exit button
    {
        //SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, transform, 1f); 
        //used the transform of a ui button so maybe that's why it didn't work 

        Debug.Log("Exit");
        Application.Quit();
    }
}
