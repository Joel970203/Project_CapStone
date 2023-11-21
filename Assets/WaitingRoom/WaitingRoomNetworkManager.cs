using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;

public class WaitingRoomNetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject User1, User2, User3;

    public void Start()
    {
        PhotonNetwork.Instantiate("WaitingRoomInfo", Vector3.zero, Quaternion.identity);
    }
}
