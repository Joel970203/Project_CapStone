using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class Boss_Info : MonoBehaviourPunCallbacks
{
    [SerializeField] private int Max_HP;
    [SerializeField] private int armor;
    [SerializeField] private int _hp;
    private int phaseNum = 1;
    public int HP // HP의 Getter 및 Setter
    {
        get { return _hp; }
        set
        {
            _hp = value;
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("SyncBossHP", RpcTarget.AllBuffered, _hp);
            }
        }
    }

    [PunRPC]
    void SyncBossHP(int newHP)
    {
        _hp = newHP;
    }

    [PunRPC]
    void SyncBossPhase(int newPhaseNum)
    {
        phaseNum = newPhaseNum;
    }

    void Start()
    {
        //HP = Max_HP;
        StartCoroutine(CheckPhase2());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_hp);
            stream.SendNext(phaseNum);
        }
        else
        {
            _hp = (int)stream.ReceiveNext();
            phaseNum = (int)stream.ReceiveNext();
        }
    }

    public void Death()
    {

    }

    IEnumerator CheckPhase2()
    {   
        while((float)HP / Max_HP > 0.99f) yield return null;

        phaseNum = 2;
        Debug.Log("2");
        StartCoroutine(CheckPhase3());
    }

    IEnumerator CheckPhase3()
    {   
        while((float)HP / Max_HP > 0.3f) yield return null;

        phaseNum = 3;
        Debug.Log("3");
        StartCoroutine(CheckPhase4());
    }

    IEnumerator CheckPhase4()
    {   
        while(HP > 0) yield return null;

        Death();
    }

    public void SetPhaseNum(int num)
    {
        phaseNum = num;
    }

    public int GetPhaseNum()
    {
        return phaseNum;
    }
}
