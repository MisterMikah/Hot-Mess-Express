using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public static Boolean prevSceneMain = false;

    [SerializeField] private AudioClip bellRingClip;
    [SerializeField] private AudioClip buttonPressClip; 

    public void PlayGame() //starts game when press start button
    {
        //play bell ring sound
        SoundFXManager.instance.PlaySoundFXClip(bellRingClip, Camera.main.transform, 1f); 
        //  - for some reason won't play bellRingClip even though it works fine when set to Exit button
        //  - currently set to button press sound in inpector
        //  - change later if figure out fix

        SceneManager.LoadScene("HotMess"); 
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

        prevSceneMain = true;
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
