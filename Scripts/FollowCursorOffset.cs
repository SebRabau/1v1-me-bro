using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursorOffset : MonoBehaviour
{
    public float xoffset = 120;
    public float yoffset = 18;

    void Update()
    {
        var pos = Input.mousePosition;
        pos.x += xoffset;
        pos.y -= yoffset;
        pos.z = 10;
        pos = Camera.main.ScreenToWorldPoint(pos);

        transform.position = pos;

        if(Input.GetMouseButtonDown(0))
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }
    }
}
