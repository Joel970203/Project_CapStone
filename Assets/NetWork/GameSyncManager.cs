using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameSyncManager : MonoBehaviourPunCallbacks
{
    public GameObject[] Characters;
    public PhotonView PV;

    public GameObject LoadingUI;
    private void Awake()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "LoadScene", true } });
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            StartCoroutine("Loading");
        }
    }

    public bool AllReady()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].CustomProperties["LoadScene"] == null)
                return false;
        }
        return true;
    }

    [PunRPC]
    public void CharacterSetting()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log(PhotonNetwork.PlayerList[i].CustomProperties["Character"]);
            switch (PhotonNetwork.PlayerList[i].CustomProperties["Character"])
            {
                case "Warrior":
                    Characters[0].SetActive(true);
                    Characters[0].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    break;
                case "Paladin":
                    Characters[1].SetActive(true);
                    Characters[1].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    break;
                case "Mage":
                    Characters[2].SetActive(true);
                    Characters[2].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    break;
                case "Healer":
                    Characters[3].SetActive(true);
                    Characters[3].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    break;
                case "Ninja":
                    Characters[4].SetActive(true);
                    Characters[4].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    break;
                case "Archer":
                    Characters[5].SetActive(true);
                    Characters[5].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    break;
            }
        }
        LoadingUI.SetActive(false);
    }

    IEnumerator Loading()
    {
        while (!AllReady())
        {
            yield return null;
        }
        PV.RPC("CharacterSetting", RpcTarget.AllBuffered);
        yield return null;
    }
}
