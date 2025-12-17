using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] private AudioClip buttonPressClip;
    [SerializeField] private AudioClip slidingPopClip;
    [SerializeField] public Slider volumeSlider; 
    public void LoadPrevScene() //return to previous screen (Main Menu or Pause Screen) when press back button
    {
        //scuffed as hell, may change later should better solution present itself

        if (MainMenuScript.prevSceneMain) //load main menu if main menu is prev scene
        {
            SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, Camera.main.transform, 1f); 
            MainMenuScript.prevSceneMain = false;
            SceneManager.LoadScene("Main Menu");
        }
        //include case for pause screen at minigame

        //include case for pause screen at runner

    }

      
    //Doesn't sound good cause it gets called every time slider value changes
    public void SliderSounds() //may delete
    {
        //make temp object that holds slider
        Debug.Log(volumeSlider.value);

        //test 
        if ((Math.Round(volumeSlider.value) % 0.2) == 0) //doesn't work bc they aren't straight but divisible 0.02 ever (you get vals like 0.01231095)
        {   //tried rounding, may have to scrap this idea

            Debug.Log("In if statement");
            SoundFXManager.instance.PlaySoundFXClip(slidingPopClip, Camera.main.transform, 1f);
        }
    }
}
