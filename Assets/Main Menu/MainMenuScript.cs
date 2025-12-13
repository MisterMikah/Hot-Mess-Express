using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame() //starts game when press start button
    {
        SceneManager.LoadScene("Scene_1"); 
        //REMINDERS:
        //      - change string name from "Scene_1" to whatever the real scene will 
        //          officially be called
        //      - in build settings make sure to add the scenes that it will navigate to 
        //             - how to get to build settings: file -> build settings
    }

    public void LoadSettingsMenu() //loads settings menu when press settings button
    {
        //pass scene number?
        SceneManager.LoadScene("Settings Menu");
    }

    public void QuitGame() //quits game when press exit button
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
