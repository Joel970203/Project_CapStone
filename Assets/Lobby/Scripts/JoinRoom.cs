using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class JoinRoom : MonoBehaviourPunCallbacks
{
    public TMP_Text RoomName;
    public TMP_Text PeopleCount;
    public void JoinToRoom()
    {
        PhotonNetwork.JoinRoom(RoomName.text);
    }
        public override void OnJoinedRoom()
    {
        Debug.Log("조인 룸");
        PhotonNetwork.LoadLevel("Waiting Room");
    }
}
