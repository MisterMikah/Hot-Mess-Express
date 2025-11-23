using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookMove : MonoBehaviour
{
    private int foodValue = 0;
    private MeshRenderer meatMat;
    private string stillCooking = "y";

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
        GetComponent<Transform>().position = new Vector3(0, .66f, 0);
        GameFlow.plateValue += foodValue;
        stillCooking = "n";
    }

    IEnumerator cookTimer()
    {
        yield return new WaitForSeconds(10);
        foodValue = 100;
        if (stillCooking == "y")
        {
            meatMat.material.color = new Color(.3f, .3f, .3f);
        }
    }
}
