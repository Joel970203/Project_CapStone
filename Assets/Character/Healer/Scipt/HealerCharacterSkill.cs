using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerCharacterSkill : Character_Skill
{
    [SerializeField]
    private Transform CastingPosition;

    [SerializeField]
    private GameObject NormalAttackObject;

    [SerializeField]
    private Transform Back;

    [SerializeField]
    private Transform Feet;

    [SerializeField]
    private GameObject QCastingEffect;

    [SerializeField]
    private GameObject QTargettingEffect;

    [SerializeField]
    private GameObject WCastingEffect;

    [SerializeField]
    private GameObject WTargettingEffect;

    [SerializeField]
    private GameObject ECastingEffect;

    [SerializeField]
    private GameObject ETargettingEffect;

    [SerializeField]
    private GameObject RCastingEffect;

    //기본 공격
    //화면상 마우스 위치로 레이캐스트를 쏴서 타격 지점을 구하고 현재 캐릭터 포지션과 타격 지점의 방향을 구해
    //해당 방향으로 생성된 오브젝트(에너지 볼트)가 이동하게 구현.
    public override void NormalAttack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << LayerMask.NameToLayer("Moveable");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                StopMove(0.5f);
                anim.SetBool("Walk", false);
                agent.ResetPath();
                ResetForward(hit.point, -50f, 0.2f);
                SettingParticle(hit.point, CastingPosition, null, null, 0.2f);
                StartCoroutine(ShootEnergyBolt(hit.point));
                anim.SetTrigger("QSkillTrigger");
                StartCoroutine(QSkillDelay(0.5f));
            }
        }
    }

    IEnumerator ShootEnergyBolt(Vector3 HitPoint)
    {
        yield return new WaitForSeconds(0.1f);
        GameObject EnergyBolt = Instantiate(NormalAttackObject, CastingPosition.position, CastingPosition.rotation);
        Rigidbody EnergyBoltRigid = EnergyBolt.GetComponent<Rigidbody>();
        EnergyBoltRigid.velocity = (HitPoint - this.transform.position).normalized * 70f;
        yield break;
    }

    //힐러 Q 스킬
    //화면상 마우스 위치로 레이캐스를 쏴서 해당 위치를 타격 지점으로 하는 빛나는 창 오브젝트를 생성.
    public override void Active_Q_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int layerMask = 1 << LayerMask.NameToLayer("Moveable");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                StopMove(1.5f);
                anim.SetBool("Walk", false);
                agent.ResetPath();
                ResetForward(hit.point, -50f, 0.2f);

                SettingParticle(hit.point, Back, QCastingEffect, QTargettingEffect, 0.2f);
                anim.SetTrigger("QSkillTrigger");
                StartCoroutine(QSkillDelay(1.5f));
                Q_Cooltime_Check = Q_Cooltime;
                Q_Skill = false;
            }
        }
    }

    IEnumerator QSkillDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetTrigger("QSkillTrigger");
        yield break;
    }

    //힐러 W 스킬. 힐링
    //화면상 마우스 위치로 레이캐스트를 쏴서 맞는 오브젝트가 플레이어인지 확인. 자신이라면 넘어가고 다른 플레이어라면
    //해당 위치로 힐링 오브젝트 생성.
    public override void Active_W_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            int layerMask = 1 << LayerMask.NameToLayer("Player");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {

                if (hit.collider.gameObject == this.gameObject)
                {
                    //Debug.Log(hit.collider.gameObject);
                }
                else
                {
                    StopMove(1.0f);
                    anim.SetBool("Walk", false);
                    agent.ResetPath();
                    ResetForward(hit.point, -50f, 0.2f);
                    SettingParticle(hit.point, CastingPosition, WCastingEffect, WTargettingEffect, 0f);
                    anim.SetTrigger("WSkillTrigger");
                    W_Cooltime_Check = W_Cooltime;
                    W_Skill = false;
                }
            }
        }
    }

    //힐러 E 스킬. 광역 버프
    //화면상 마우스 위치로 레이캐스트를 쏴서 해당 위치를 캐릭터가 바라보게 하고, 애니메이션과 파티클 재생.
    public override void Active_E_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int layerMask = 1 << LayerMask.NameToLayer("Moveable");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                StopMove(3f);
                anim.SetBool("Walk", false);
                agent.ResetPath();
                ResetForward(hit.point, 0f, 0.2f);
                SettingParticle(hit.point, Feet, ECastingEffect, null, 1f);
                anim.SetTrigger("ESkillTrigger");
                E_Cooltime_Check = E_Cooltime;
                E_Skill = false;
            }
        }
    }

    //힐러 R 스킬. 부활
    //화면상 마우스 위치로 레이캐스트를 쏴서 맞은 오브젝트의 레이어를 분석. Layer가 Grave라면 동작.
    //애니메이션 재생하며 Grave에 기록된 플레이어를 활성화하고, Grave 오브젝트 비활성화.
    public override void Active_R_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            int layerMask = 1 << LayerMask.NameToLayer("Grave");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                StopMove(3f);
                anim.SetBool("Walk", false);
                agent.ResetPath();
                ResetForward(hit.point, 60f, 0.2f);
                SettingParticle(hit.point, Feet, RCastingEffect, null, 2.8f);
                anim.SetTrigger("RSkillTrigger");

                StartCoroutine(RSkillDelay(4f));

                StartCoroutine(TriggerRivival(hit.collider.gameObject, 4f));
                R_Cooltime_Check = R_Cooltime;
                R_Skill = false;
            }
        }
    }
    IEnumerator RSkillDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetTrigger("RSkillTrigger");
        yield break;
    }

    IEnumerator TriggerRivival(GameObject Grave, float delay)
    {
        yield return new WaitForSeconds(delay);
        Grave.GetComponent<RivivalCharacter>().rivival();
        yield break;
    }
}
