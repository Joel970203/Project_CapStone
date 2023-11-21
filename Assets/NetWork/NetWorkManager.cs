using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetWorkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
     void Awake() 
    {
        Screen.SetResolution(1280, 720, false);
        PhotonNetwork.ConnectUsingSettings();
    }


   public override void OnConnectedToMaster() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 3 }, null);

   public override void OnJoinedRoom() 
   {
        //PhotonNetwork.Instantiate("Ninja", new Vector3(858, 55, 896), Quaternion.identity);
   }
}