using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class ReadyOrStart : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(OnClickButton);
    }

    // Update is called once per frame
    void OnClickButton()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObject in playerObjects)
        {
            PhotonView photonView = playerObject.GetPhotonView();

            if (photonView.IsMine)
            {
                playerObject.GetComponent<WaitingRoomInfo>().PressReadyOrStart();

                return;
            }
        }
    }
}
