using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectManager : Photon.MonoBehaviour
{
    private void OnDisconnectFromPhoton()
    {
        PhotonNetwork.LoadLevel(0);
    }
}
