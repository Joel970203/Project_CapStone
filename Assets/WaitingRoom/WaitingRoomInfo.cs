using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WaitingRoomInfo : MonoBehaviour
{
    public PhotonView PV;
    void Start()
    {
        if (PV.IsMine)
        {
            StartCoroutine(DelayedRPC());
        }
    }


    IEnumerator DelayedRPC()
    {
        yield return new WaitForSeconds(0.05f);
        PV.RPC("SetUserInfoPosition", RpcTarget.AllBuffered);
    }
    [PunRPC]
    void SetUserInfoPosition()
    {
        if (GameObject.Find("User1").transform.childCount == 0)
            this.transform.SetParent(GameObject.Find("User1").transform);
        else if (GameObject.Find("User2").transform.childCount == 0)
            this.transform.SetParent(GameObject.Find("User2").transform);
        else if (GameObject.Find("User3").transform.childCount == 0)
            this.transform.SetParent(GameObject.Find("User3").transform);
        else
            Debug.Log("에러 발생");

        this.GetComponent<RectTransform>().offsetMin = new Vector2(10f, 10f); // left, bottom
        this.GetComponent<RectTransform>().offsetMax = new Vector2(-10f, -10f); // right, top
    }
}
