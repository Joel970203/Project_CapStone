using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja_Clone : Character_Skill
{
    [SerializeField]
    private GameObject weapon;
    bool isAttacking = false;

    public override void Active_Base_Attack()
    {
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")) && agent.remainingDistance < 0.1f)
        {
            agent.ResetPath();
            anim.SetBool("Walk", false);

            if (!isAttacking)
            {
                isAttacking = true;
                StartCoroutine(SetActiveBaseAttack());
            }
        }
    }

    IEnumerator SetActiveBaseAttack()
    {
        agent.velocity = Vector3.zero;
        agent.isStopped = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion finalRotation = Quaternion.Euler(0, 55f, 0) * targetRotation;
            anim.transform.rotation = finalRotation;
            anim.SetTrigger("Base Attack");

            yield return new WaitForSeconds(1.0f); // 애니메이션 시간에 맞게 조절

            agent.isStopped = false;
            isAttacking = false;
        }
    }

    public override void Active_Q_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {       
            anim.SetBool("Walk", false);
            agent.ResetPath();
            GameObject boss = GameObject.FindWithTag("Boss");
            if (boss != null)
            {
                // 보스와 플레이어 간의 거리와 방향 계산
                Vector3 bossPosition = boss.transform.position;
                Vector3 playerPosition = transform.position;
                Vector3 bossDirection = (bossPosition - playerPosition).normalized;
                float skillDistance = 50.0f; // 이동할 거리 (적절한 값 찾기)

                // 보스가 바라보는 방향의 뒤로 이동 위치 계산
                Vector3 movePosition = bossPosition + bossDirection * skillDistance;

                // 이동 위치로 플레이어 이동
                transform.position = movePosition;

                // 보스를 향하도록 회전
                transform.LookAt(boss.transform);
                
                anim.SetTrigger("Q");
            }
        }
    }

    public override void Active_W_Skill()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                anim.SetBool("Walk", false);
                agent.ResetPath();
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // 시계 방향으로 55도 회전
                Quaternion finalRotation = Quaternion.Euler(0, 90f, 0) * targetRotation;

                // 공격 모션 방향을 설정하고 공격 애니메이션 실행
                anim.transform.rotation = finalRotation;

                anim.SetTrigger("W");
                StartCoroutine(SpawnDelay(0.5f, 0.35f, 3)); 
            }
        }
    }

    private IEnumerator SpawnDelay(float initialDelay, float interval, int spawnCount)
    {
        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < spawnCount; i++)
        {
            yield return new WaitForSeconds(interval);

            Vector3 spawnPosition = transform.position + transform.forward * 10f;
            spawnPosition.y += 20f;

            Quaternion particleRotation = Quaternion.Euler(0f, 60f, 0f);
        }
    }

    public override void Active_E_Skill()
    {
        // 스킬 E 실행
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
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
                Quaternion finalRotation = Quaternion.Euler(0, 55f, 0) * targetRotation;
                transform.rotation = finalRotation;
                anim.SetTrigger("E");
            }
        }
    }

    
    public override void Active_R_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
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
                Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);
                transform.rotation = targetRotation;

                anim.SetTrigger("R");
            }
        }
    }
}
