using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPlace : MonoBehaviour
{
    public Transform cloneObj;
    public int foodValue;
    public AudioClip placeClip;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (gameObject.name == "burger_bread_1")
            Instantiate(cloneObj, new Vector3(3, .10f, -2), cloneObj.rotation);

        if (gameObject.name == "burger_bread_2")
            Instantiate(cloneObj, new Vector3(3, .60f, -2), cloneObj.rotation);

        if (gameObject.name == "cheese_slice")
            Instantiate(cloneObj, new Vector3(3, .62f, -2), cloneObj.rotation);

        if (gameObject.name == "lettuce_slice")
            Instantiate(cloneObj, new Vector3(3, .65f, -2), cloneObj.rotation);

        if (gameObject.name == "onion_slice")
            Instantiate(cloneObj, new Vector3(3, .66f, -2), cloneObj.rotation);

        if (gameObject.name == "tomato_slice")
            Instantiate(cloneObj, new Vector3(3, .67f, -2), cloneObj.rotation);

        //place food sound
        SoundFXManager.instance.PlaySoundFXClip(placeClip, transform, 1f);

        GameFlow.plateValue += foodValue;
        Debug.Log(GameFlow.plateValue + "  " + GameFlow.orderValue);
    }
}
