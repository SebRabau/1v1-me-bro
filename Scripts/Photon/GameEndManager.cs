using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndManager : Photon.MonoBehaviour
{
    public void leaveScene()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
    }
    
    public void gameEnd()
    {
        StartCoroutine(endTimer());
    }

    public void deletePlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for(int i=0; i<players.Length; i++)
        {
            PhotonNetwork.Destroy(players[i]);
        }

        StartCoroutine(timerWait());
    }

    public void playerToMenu(GameObject player)
    {
        PhotonNetwork.Destroy(player);

        StartCoroutine(timerWait());
    }

    IEnumerator endTimer()
    {
        yield return new WaitForSeconds(10f);

        deletePlayers();
    }

    IEnumerator timerWait()
    {
        yield return new WaitForSeconds(3f);

        leaveScene();
    }
}
