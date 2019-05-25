using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pinger : Photon.MonoBehaviour { 

    Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void FixedUpdate()
    {
        text.text = PhotonNetwork.networkingPeer.RoundTripTime.ToString();
    }
}
