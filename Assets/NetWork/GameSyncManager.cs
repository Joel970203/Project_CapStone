using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.TextCore.Text;

public class GameSyncManager : MonoBehaviourPunCallbacks
{
    public GameObject[] Characters;
    public PhotonView PV;

    public GameObject LoadingUI;

    public GameObject myCharacter;

    public GameObject MainCamera;

    public Image HealthGlobe;

    public GameObject SkillIcon;
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
        GameObject temp = Characters[0];
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log(PhotonNetwork.PlayerList[i].CustomProperties["Character"]);
            switch (PhotonNetwork.PlayerList[i].CustomProperties["Character"])
            {
                case "Warrior":
                    Characters[0].SetActive(true);
                    Characters[0].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    temp = Characters[0];
                    break;
                case "Paladin":
                    Characters[1].SetActive(true);
                    Characters[1].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    temp = Characters[1];
                    break;
                case "Mage":
                    Characters[2].SetActive(true);
                    Characters[2].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    temp = Characters[2];
                    break;
                case "Healer":
                    Characters[3].SetActive(true);
                    Characters[3].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    temp = Characters[3];
                    break;
                case "Ninja":
                    Characters[4].SetActive(true);
                    Characters[4].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    temp = Characters[4];
                    break;
                case "Archer":
                    Characters[5].SetActive(true);
                    Characters[5].GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[i]);
                    temp = Characters[5];
                    break;
            }
            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
            {
                myCharacter = temp;
                MainCamera.GetComponent<CameraMove>().Character = myCharacter;
                myCharacter.GetComponent<Character_Info>().HealthGlobe = HealthGlobe;
                switch (myCharacter.name)
                {
                    case "Warrior":
                        break;
                    case "Paladin":
                        break;
                    case "Mage":
                        for (int j = 0; j < 4; j++)
                        {
                            SkillIcon.transform.GetChild(j).GetComponent<Image>().sprite = myCharacter.GetComponent<MageCharacterSkill>().SkillIcons[j];
                        }
                        break;
                    case "Healer":
                        for (int j = 0; j < 4; j++)
                        {
                            SkillIcon.transform.GetChild(j).GetComponent<Image>().sprite = myCharacter.GetComponent<HealerCharacterSkill>().SkillIcons[j];
                        }
                        break;
                    case "Ninja":
                        break;
                    case "Archer":
                        break;
                }
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
