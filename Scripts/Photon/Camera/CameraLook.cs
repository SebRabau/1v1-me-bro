using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : Photon.MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (!photonView.isMine)
        {
            return;
        }

        transform.LookAt(target);
    }
}
