using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPlace : MonoBehaviour
{
    public Transform cloneObj;
    public int foodValue;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (gameObject.name == "burger_bread_1")
            Instantiate(cloneObj, new Vector3(0, .10f, 0), cloneObj.rotation);

        if (gameObject.name == "burger_bread_2")
            Instantiate(cloneObj, new Vector3(0, .60f, 0), cloneObj.rotation);

        if (gameObject.name == "cheese_slice")
            Instantiate(cloneObj, new Vector3(0, .62f, 0), cloneObj.rotation);

        if (gameObject.name == "lettuce_slice")
            Instantiate(cloneObj, new Vector3(0, .63f, 0), cloneObj.rotation);

        if (gameObject.name == "onion_slice")
            Instantiate(cloneObj, new Vector3(0, .64f, 0), cloneObj.rotation);

        if (gameObject.name == "tomato_slice")
            Instantiate(cloneObj, new Vector3(0, .65f, 0), cloneObj.rotation);

        GameFlow.plateValue += foodValue;
        Debug.Log(GameFlow.plateValue + "  " + GameFlow.orderValue);
    }
}
