using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomList : MonoBehaviourPunCallbacks
{   
    public GameObject RoomPrefab;
    public GameObject RoomListContents;
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int RoomCount = RoomListContents.transform.childCount;
        for(int i=0;i<RoomCount;i++)
        {
            RoomListContents.transform.GetChild(0).gameObject.SetActive(false);
            Destroy(RoomListContents.transform.GetChild(0).gameObject);
        }
        for(int i=0;i<roomList.Count;i++)
        {
            GameObject Room = Instantiate(RoomPrefab,Vector3.zero,Quaternion.identity,RoomListContents.transform);
            Room.GetComponent<JoinRoom>().RoomName.text=roomList[i].Name;
            Room.GetComponent<JoinRoom>().PeopleCount.text=roomList[i].PlayerCount.ToString()+"/3";
        }
    }
}
