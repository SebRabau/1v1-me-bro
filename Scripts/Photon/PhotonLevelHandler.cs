using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonLevelHandler : Photon.MonoBehaviour
{
    public PhotonButtons buttons;
    public GameObject myPlayer;

    public static GameObject player1, player2;

    private void Awake()
    {
        DontDestroyOnLoad(this.transform);

        if(FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public void loadGame()
    {
        AudioSource aS = FindObjectOfType<AudioSource>();
        
        if(aS != null)
        {
            aS.Stop();
        }

        PhotonNetwork.LoadLevel("Game");
    }

    public void joinOrCreateNewRoom()
    {
        if (buttons.joinOrCreateRoom.text.Length >= 1)
        {
            PhotonNetwork.JoinOrCreateRoom(buttons.joinOrCreateRoom.text, new RoomOptions() { MaxPlayers = 2 }, TypedLobby.Default);
        }
    }

    private void OnJoinedRoom()
    {
        loadGame();
        Debug.Log("We are Connected to the Room");
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Game")
        {
            spawnPlayer();
        }
    }

    private void spawnPlayer()
    {
        if (PhotonNetwork.room.playerCount == 1)
        {
            if(GameObject.Find("Player 1") == null)
            {
                player1 = PhotonNetwork.Instantiate(myPlayer.name, myPlayer.transform.position, myPlayer.transform.rotation, 0) as GameObject;
                player1.name = "Player 1";
            } else
            {
                Debug.Log("Tried to spawn duplicate player 1");
            }
            
        }
        else
        {
            if(GameObject.Find("Player 2") == null)
            {
                player2 = PhotonNetwork.Instantiate(myPlayer.name, myPlayer.transform.position, myPlayer.transform.rotation, 0) as GameObject;
                player2.name = "Player 2";
            }
            else
            {
                Debug.Log("Tried to spawn duplicate player 2");
            }
            
        }
    }

    private void OnDisconnectFromPhoton()
    {
        Destroy(gameObject);
    }
}
