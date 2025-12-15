using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookMove : MonoBehaviour
{
    private int foodValue = 0;
    private MeshRenderer meatMat;
    private string stillCooking = "y";
    public AudioClip grillingClip;


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

        //destroy audio for grilling when meat off the pan
        SoundFXManager.instance.DestroyLoopingFXClip();
    }

    IEnumerator cookTimer()
    {
        //start looping audio for grilling
        SoundFXManager.instance.PlayLoopingFXClip(grillingClip, transform, 0.5f);

        yield return new WaitForSeconds(10);
        foodValue = 100;
        if (stillCooking == "y")
        {
            meatMat.material.color = new Color(.3f, .3f, .3f);
        }
    }
}
