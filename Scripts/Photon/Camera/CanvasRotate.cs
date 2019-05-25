using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRotate : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
            cam = GameObject.FindObjectOfType<Camera>();    
    }


    // Update is called once per frame
    void Update()
    {
        if (cam != null)
        {
            transform.LookAt(cam.transform);
        }         
    }
}
