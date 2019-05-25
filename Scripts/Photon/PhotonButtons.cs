using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PhotonButtons : MonoBehaviour
{
    public PhotonLevelHandler handler;

    public InputField joinOrCreateRoom;

    public void onClickJoinRoom()
    {
        handler.joinOrCreateNewRoom();
    }

    
}
