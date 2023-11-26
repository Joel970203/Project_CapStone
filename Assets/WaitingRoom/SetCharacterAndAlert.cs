using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class SetCharacterAndAlert : MonoBehaviourPunCallbacks
{
    public string CharacterName;
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(OnClickButton);
        this.gameObject.GetComponent<Button>().interactable=false;
    }

    void OnClickButton()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObject in playerObjects)
        {
            PhotonView photonView = playerObject.GetPhotonView();

            if (photonView.IsMine)
            {
                if(!playerObject.GetComponent<WaitingRoomInfo>().CharacterOwnerCheck(CharacterName))
                {
                    playerObject.GetComponent<WaitingRoomInfo>().SetCharacterNameAndAlert(CharacterName);
                }
                return;
            }
        }
    }
}
