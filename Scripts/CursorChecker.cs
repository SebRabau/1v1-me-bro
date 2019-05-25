using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChecker : MonoBehaviour
{
    void Update()
    {
        if(Cursor.lockState == CursorLockMode.Confined || Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
