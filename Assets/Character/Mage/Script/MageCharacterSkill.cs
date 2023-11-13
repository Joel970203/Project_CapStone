using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MageCharacterSkill : Character_Skill
{
    [SerializeField]
    private Transform CastingPosition;

    [SerializeField]
    private GameObject NormalAttackObject;

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
    [SerializeField]
    private GameObject RCastingEffect2;

    [SerializeField]
    private GameObject RTargettingEffect;

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
                StopMove(1f);
                anim.SetBool("Walk", false);
                agent.ResetPath();
                ResetForward(hit.point, -50f, 0.2f);
                SettingParticle(hit.point, CastingPosition, QCastingEffect, null, 0.2f);
                StartCoroutine(ShootEnergyBolt(hit.point));
                anim.SetTrigger("QSkillTrigger");
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

    //메이지 Q스킬 발동 함수. 벼락 생성
    //화면상 마우스 위치로 레이캐스트를 쏴서 타격 위치를 구해 오브젝트 해당 위치로 생성.
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
                SettingParticle(hit.point, CastingPosition, QCastingEffect, QTargettingEffect, 0.2f);
                anim.SetTrigger("QSkillTrigger");
                Q_Cooltime_Check = Q_Cooltime; // 1
                Q_Skill = false; // 2 
            }
        }
    }

    //메이지 W 스킬. 텔레포트
    //화면상 마우스 위치로 레이캐스트를 쏴서 해당 위치로 캐릭터가 이동하는 함수.
    public override void Active_W_Skill()
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
                ResetForward(hit.point, -50f, 0.2f);

                Vector3 Temp = this.transform.position;
                Temp.y = 50.5f;
                Transform Position = this.transform;
                Position.position = Temp;
                SettingParticle(hit.point, Position, WCastingEffect, WTargettingEffect, 0f);

                StartCoroutine(Teleport(hit.point));
                anim.SetTrigger("QSkillTrigger");
                W_Cooltime_Check = W_Cooltime;
                W_Skill = false;
            }
        }
    }

    IEnumerator Teleport(Vector3 HitPoint)
    {
        yield return new WaitForSeconds(1.2f);

        Vector3 WarpPosition = new Vector3(HitPoint.x, 60f, HitPoint.z);

        agent.Warp(WarpPosition);
        yield break;
    }
    
    //메이지 E 스킬. 지속딜 주는 불기둥 소환
    //화면상 마우스 위치로 레이캐스트를 쏴서 해당 위치에 불기둥 오브젝트 생성하는 함수.
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
                ResetForward(hit.point, 35f, 0.2f);
                SettingParticle(hit.point, CastingPosition, ECastingEffect, ETargettingEffect, 1f);
                anim.SetTrigger("ESkillTrigger");
                E_Cooltime_Check = E_Cooltime;
                E_Skill = false;
            }
        }
    }

    //메이지 R스킬
    //화면상 마우스 위치에 레이캐스트를 쏴서 해당 위치를 타격 지점으로 하고, 메테오 오브젝트 소환.
    //메테오 오브젝트튼 타격 위치로 이동함. 일정 시간 후 (땅에 박혔을 때) 폭발하면서 주변에 데미지를 줌.
    public override void Active_R_Skill()
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
                ResetForward(hit.point, 25f, 0.2f);
                SettingParticle(hit.point, CastingPosition, RCastingEffect, RTargettingEffect, 2.8f);
                Vector3 Temp = this.transform.position;
                Temp.y = 50.5f;
                Transform Position = this.transform;
                Position.position = Temp;
                SettingParticle(hit.point, Position, RCastingEffect2, null, 2.8f);
                anim.SetTrigger("RSkillTrigger");

                StartCoroutine(RSkillDelay(4f));
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
}
