using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    public GameObject igMenu;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !igMenu.active)
        {
            igMenu.GetComponent<InGameSettings>().updateSettings();
            Cursor.lockState = CursorLockMode.None;
            igMenu.SetActive(true);
        } else if(Input.GetKeyDown(KeyCode.Escape) && igMenu.active)
        {
            igMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
