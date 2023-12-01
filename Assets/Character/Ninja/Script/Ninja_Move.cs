using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ninja_Move : MonoBehaviourPunCallbacks
{
    UnityEngine.AI.NavMeshAgent agent;
    public Animator anim;
    PhotonView PV;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();

        // 다른 플레이어일 경우, 애니메이션 동기화를 위한 PhotonAnimatorView 비활성화
        if (!PV.IsMine)
        {
            PhotonAnimatorView photonAnimatorView = GetComponent<PhotonAnimatorView>();
            if (photonAnimatorView != null)
            {
                Destroy(photonAnimatorView);
            }
        }
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            PhotonAnimatorView photonAnimatorView = GetComponent<PhotonAnimatorView>();
            if (photonAnimatorView != null)
            {
                Destroy(photonAnimatorView);
            }
            return;
        } // 다른 플레이어일 경우, 이후 코드를 실행하지 않음

        HandleMovement();
        HandleAttack();
        HandleDodge();
    }

    void HandleMovement()
    {
        if (Input.GetMouseButtonDown(1) && (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
                anim.SetBool("Walk", true);
                PV.RPC("SetWalkAnimationState", RpcTarget.Others, true);
            }
        }
        else if (agent.remainingDistance < 0.1f)
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            PV.RPC("SetWalkAnimationState", RpcTarget.Others, false);
        }
    }

    void HandleAttack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            PV.RPC("SetWalkAnimationState", RpcTarget.Others, false);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Stand Up"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                anim.ResetTrigger("Hit");
            }
        }
    }

    void HandleDodge()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Stand Up") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 dodge_Direction = new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position;

                    anim.transform.forward = dodge_Direction;

                    if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x >= 0.5)
                    {
                        anim.SetBool("Right_Dodge", true);
                    }
                    else
                    {
                        anim.SetBool("Right_Dodge", false);
                    }
                    anim.SetTrigger("Dodge");
                    PV.RPC("PerformDodgeAnimation", RpcTarget.All);
                }
            }
        }
    }

    [PunRPC]
    void SetWalkAnimationState(bool isWalking)
    {
        anim.SetBool("Walk", isWalking);
    }

    [PunRPC]
    void PerformDodgeAnimation()
    {
        anim.SetTrigger("Dodge");
    }
}
