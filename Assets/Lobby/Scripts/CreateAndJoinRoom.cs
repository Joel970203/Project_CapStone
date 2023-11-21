using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    public TMP_InputField RoomName;
    /*
    public override void OnEnable() 
    {
        RoomName.text=string.Empty;
    }
    */
    public void CreateRoom()
    {
        Debug.Log("방 만드는중");
        PhotonNetwork.CreateRoom(RoomName.text, new RoomOptions { MaxPlayers = 3 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("조인 룸");
        PhotonNetwork.LoadLevel("Waiting Room");
    }
}
