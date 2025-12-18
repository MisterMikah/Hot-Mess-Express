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

    public static int settingsPrevScene;
    public void LoadPrevScene() //return to previous screen (Main Menu or Pause Screen) when press back button
    {
        
        if (settingsPrevScene == 0) //prev screen is Main Menu
        {
            SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, Camera.main.transform, 1f); 
            SceneManager.LoadScene("Main Menu");
        }
        else if (settingsPrevScene == 1) //prev screen is Minigame
        {
            SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, Camera.main.transform, 1f); 
            SceneManager.LoadScene("HotMess"); 
        }
        else if (settingsPrevScene == 2) //prev screen is Runner
        {
            SoundFXManager.instance.PlaySoundFXClip(buttonPressClip, Camera.main.transform, 1f); 
            SceneManager.LoadScene("Scene_1");
        }
        

        

    }

    /*
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
    */
}
