using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsScript : MonoBehaviour
{
    public void LoadPrevScene() //return to previous screen (Main Menu or Pause Screen) when press back button
    {
        //scuffed as hell, may change later should better solution present itself

        if (MainMenuScript.prevSceneMain) //load main menu if main menu is prev scene
        {
            MainMenuScript.prevSceneMain = false;
            SceneManager.LoadScene("Main Menu");
        }
        //include case for pause screen at minigame

        //include case for pause screen at runner

    }

}
