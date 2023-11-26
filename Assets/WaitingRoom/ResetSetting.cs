using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class ResetSetting : MonoBehaviourPunCallbacks
{
    public GameObject Characters;
    private string HexColor = "#344E84";
    public Button CheckButton;
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(OnClickButton);
    }

    void OnClickButton()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObject in playerObjects)
        {
            PhotonView photonView = playerObject.GetPhotonView();

            if (photonView.IsMine)
            {
                if((string)PhotonNetwork.LocalPlayer.CustomProperties["Character"]=="")
                {
                    Color NewColor;
                    if (ColorUtility.TryParseHtmlString(HexColor, out NewColor))
                    {
                        CheckButton.interactable = false;
                        CheckButton.image.color = NewColor;
                        for(int i=0;i<Characters.transform.childCount;i++)
                        {
                            Characters.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.LogError("Invalid HEXADECIMAL color: " + HexColor);
                    }
                }
                return;
            }
        }
    }
}
