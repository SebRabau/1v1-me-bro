using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhotonConnect : MonoBehaviour
{
    public string versionName = "0.1";

    public GameObject secView1, secView2, secView3;

    public void connectToPhoton()
    {
        if(PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
        PhotonNetwork.ConnectUsingSettings(versionName);

        Debug.Log("Connecting to Photon");
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);

        Debug.Log("Connected to Master");
    }

    private void OnJoinedLobby()
    {
        secView1.SetActive(false);
        secView2.SetActive(true);

        Debug.Log("On Joined Lobby");
    }

    private void OnDisconnectedFromPhoton()
    {
        if(secView1.active)
        {
            secView1.SetActive(false);
        }
        if(secView2.active)
        {
            secView2.SetActive(false);
        }

        secView3.SetActive(true);

        Debug.Log("Disconnected from Photon Services");
    }
    
}
