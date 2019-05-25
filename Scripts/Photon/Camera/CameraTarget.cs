using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : Photon.MonoBehaviour
{
    float my;

    void Update()
    {
        if (!photonView.isMine)
        {
            return;
        }

        my = Input.GetAxis("Mouse Y") * Time.deltaTime * 50.0f;

        if(transform.position.y < 20 && my > 0)
        {
            transform.Translate(0, my, 0);
        } else if(transform.position.y > -20 && my < 0)
        {
            transform.Translate(0, my, 0);
        } 
        
    }
}
