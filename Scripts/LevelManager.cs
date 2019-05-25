using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject player;
    public GameObject holder;

    public void toLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void toSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void toInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }

    public void toMenuGame()
    {
        Destroy(GameObject.FindGameObjectWithTag("PlayerHolder"));
        FindObjectOfType<GameEndManager>().playerToMenu(player);
    }

    public void toMenu()
    {
        if(holder != null)
        {
            Destroy(holder);
        }
        
        SceneManager.LoadScene("Menu");
    }

    public void exit()
    {
        Application.Quit();
    }
}
