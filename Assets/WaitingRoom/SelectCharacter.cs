using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class SelectCharacter : MonoBehaviourPunCallbacks
{
    public Button CheckButton;
    private string HexColor = "#344E84";
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
                if (!playerObject.GetComponent<WaitingRoomInfo>().CharacterOwnerCheck(this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text))
                {
                    CheckButton.interactable = true;
                    CheckButton.gameObject.GetComponent<SetCharacterAndAlert>().CharacterName = this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
                    CheckButton.image.color = Color.black;
                }
                else
                {
                    Color NewColor;
                    if (ColorUtility.TryParseHtmlString(HexColor, out NewColor))
                    {
                        CheckButton.interactable = false;
                        CheckButton.image.color = NewColor;
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
