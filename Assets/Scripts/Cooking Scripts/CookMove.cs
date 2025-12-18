using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is for cooking the patty in the pan for 5 seconds
//and then moves it to the plate when the player clicks the patty in the pan when it has changed color.

public class CookMove : MonoBehaviour
{
    private int foodValue = 0;
    private MeshRenderer meatMat;
    private string stillCooking = "y";
    public AudioClip grillingClip;
    public AudioClip placeClip;

    void Start()
    {
        meatMat = GetComponent<MeshRenderer>();
        StartCoroutine(cookTimer());
    }

    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        GetComponent<Transform>().position = new Vector3(3, .50f, -2);
        GameFlow.plateValue += foodValue;
        stillCooking = "n";

        if (SoundFXManager.instance.audioSource != null) {
            SoundFXManager.instance.DestroyLoopingFXClip(); //destroy audio for grilling when meat off the pan
        }

        //play place sound effect when put on plate
        SoundFXManager.instance.PlaySoundFXClip(placeClip, transform, 1f);
    }

    IEnumerator cookTimer()
    {
        //start looping audio for grilling
        SoundFXManager.instance.PlayLoopingFXClip(grillingClip, transform, 0.5f);

        yield return new WaitForSeconds(5);
        foodValue = 100;
        if (stillCooking == "y")
        {
            meatMat.material.color = new Color(.3f, .3f, .3f);
        }
    }
}
