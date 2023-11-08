using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class Archer_Skill : Character_Skill
{  
    public override void Active_Base_Attack()
    {
        //좌클릭 누르고있으면 setbool (Aiming) true고 좌클릭이 끝날때 트리거로 shot 발동 해서 'shot' 애니메이션 실행
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")|| anim.GetCurrentAnimatorStateInfo(0).IsName("Aim walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPosition = hit.point;
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0; 
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
            anim.SetTrigger("Shot");
        }
    }

    }
      
    public override void Active_Base_Attack()
    {   
        //좌클릭 누르고있으면 setbool (Aiming) true고 좌클릭이 끝날때 트리거로 shot 발동 해서 'shot' 애니메이션 실행
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")|| anim.GetCurrentAnimatorStateInfo(0).IsName("Aim walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPosition = hit.point;
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0; 
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
            anim.SetTrigger("Shot");
        }
    }

    public virtual void Aim_Attack()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {
            return; // Idle 또는 Walk 애니메이션 중일 때만 공격 가능하도록
        }
        anim.SetTrigger("Shot"); // Shot 애니메이션을 트리거합니다.
    }
    public override void Active_Q_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
        || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {       
            anim.SetBool("Walk", false);
            agent.ResetPath();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPosition = hit.point;
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0; 
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
            anim.SetTrigger("Q"); // 덫 
        }
    }
    public override void Active_W_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
        || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            anim.SetTrigger("W"); 
            /*
                헤이스트 작업
            
        }
    }
    public virtual void Active_E_Skill() 
    {  
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
        || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {
             anim.SetBool("Walk", false);
            agent.ResetPath();
            anim.SetTrigger("E");
        }    
    }
    public virtual void Active_R_Skill() 
    {
        agent.ResetPath();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
        || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                anim.SetBool("Walk", false);
                agent.ResetPath();
    
                // 커서방향으로 시전 
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0; // y축고정
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
              
                anim.SetTrigger("R");
            }
        }
    }
}
*/