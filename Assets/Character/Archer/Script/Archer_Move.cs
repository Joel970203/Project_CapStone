using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Archer_Move : MonoBehaviour 
{
    UnityEngine.AI.NavMeshAgent agent;
    Animator anim;

    private float originalSpeed;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        originalSpeed = agent.speed; 
    }

    // Update is called once per frame
       void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (anim.GetBool("Aiming")) 
            {
                agent.ResetPath();
                anim.SetBool("Aiming", false);
                agent.speed = originalSpeed;
            }
            else
            {
                agent.ResetPath();
                anim.SetBool("Aiming", true);
                agent.speed = originalSpeed * 0.1f;
            }
        }

        //마우스 오른쪽 클릭. Idle 상태거나 Walk 상태일 때만 이동 명령 가능
        if (Input.GetMouseButtonDown(1) && ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")) 
        || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk")))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
                anim.SetBool("Walk", true);
            }
        }
        else if (agent.remainingDistance < 0.1f)
        {
            
            anim.SetBool("Walk", false);
            agent.ResetPath();
        }


        //공격 받는다면 이동 취소 후 Walk 상태 초기화
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            
            anim.SetBool("Walk", false);
            agent.ResetPath();
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Stand Up"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                anim.ResetTrigger("Hit");
            }
        }

        //스페이스바 입력. 회피
        if (Input.GetButtonDown("Jump"))
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Stand Up") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // 마우스 바라보는 방향 구함. 마우스 위치에서 자신 위치 빼기.
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

                // Dodge 애니메이션 실행
                anim.SetTrigger("Dodge");

                // NavMeshAgent를 초기화하고 제자리로 돌리기
                agent.ResetPath(); // NavMeshAgent의 경로 초기화
        
                }
            }
        }
    }
}