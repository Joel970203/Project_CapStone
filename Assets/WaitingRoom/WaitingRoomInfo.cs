using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

using Hashtable = ExitGames.Client.Photon.Hashtable;
//using Microsoft.Unity.VisualStudio.Editor;

using UnityEngine.UI;
using ExitGames.Client.Photon.StructWrapping;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class WaitingRoomInfo : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public Sprite[] Portraits;

    public GameObject ReadyUI;
    void Start()
    {
        if (PV.IsMine)
        {
            StartCoroutine(DelayedRPC());
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "isReady", false }, { "Character", "" }, { "IsReady", false } });
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && PV.IsMine)
        {

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

    public bool CharacterOwnerCheck(string input)
    {
        if (PV.IsMine)
        {
            bool check = false;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if ((string)PhotonNetwork.PlayerList[i].CustomProperties["Character"] == input)
                {
                    check = true;
                }
            }
            return check;
        }
        return true;
    }

    public void SetCharacterNameAndAlert(string input)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "Character", input } });
        PV.RPC("AlertCharacterName", RpcTarget.AllBuffered, input);
    }

    [PunRPC]
    void AlertCharacterName(string input)
    {
        this.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = input;

        switch (input)
        {
            case "Warrior":
                this.transform.GetChild(0).GetComponent<Image>().sprite = Portraits[0];
                break;

            case "Paladin":
                this.transform.GetChild(0).GetComponent<Image>().sprite = Portraits[1];
                break;
            case "Mage":
                this.transform.GetChild(0).GetComponent<Image>().sprite = Portraits[2];
                break;
            case "Healer":
                this.transform.GetChild(0).GetComponent<Image>().sprite = Portraits[3];
                break;
            case "Ninja":
                this.transform.GetChild(0).GetComponent<Image>().sprite = Portraits[4];
                break;
            case "Archer":
                this.transform.GetChild(0).GetComponent<Image>().sprite = Portraits[5];
                break;
        }
        this.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        this.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    public void PressReadyOrStart()
    {
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["Character"] == "")
        {

        }
        else
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                bool check = true;
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    if ((bool)PhotonNetwork.PlayerList[i].CustomProperties["IsReady"] == false)
                    {
                        if (PhotonNetwork.PlayerList[i].IsMasterClient)
                        {

                        }
                        else
                        {
                            check = false;
                        }
                    }
                }
                if (check)
                {
                    Debug.Log("AllReady Start");
                    PV.RPC("StartAndLoadScene", RpcTarget.AllViaServer);
                }
                else
                {
                    Debug.Log("NotReady");
                }
            }
            else
            {
                PV.RPC("AlertReady", RpcTarget.AllBuffered);
            }
        }

    }

    [PunRPC]
    void AlertReady()
    {

        if (ReadyUI.activeSelf)
        {
            ReadyUI.SetActive(false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "IsReady", false } });
        }
        else
        {
            ReadyUI.SetActive(true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "IsReady", true } });
        }
    }

    [PunRPC]
    void StartAndLoadScene()
    {
        SceneManager.LoadScene("Game");
    }
}
