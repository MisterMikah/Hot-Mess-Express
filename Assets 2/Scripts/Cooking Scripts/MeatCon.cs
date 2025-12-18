using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this moves the patty to the pan

public class MeatCon : MonoBehaviour
{
    public Transform cloneObj;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (gameObject.name == "burger_patty")
            Instantiate(cloneObj, new Vector3(1, .1f, -2), cloneObj.rotation);
    }
}
