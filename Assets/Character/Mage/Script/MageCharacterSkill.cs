using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MageCharacterSkill : Character_Skill
{
    [SerializeField]
    private Transform CastingPosition;

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
    public override void Active_Q_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ResetForward(hit.point, -50f, 0.2f);
                SettingParticle(hit.point, CastingPosition, QCastingEffect, QTargettingEffect, 0.2f);
                anim.SetTrigger("QSkillTrigger");
            }

        }
    }

    public override void Active_W_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ResetForward(hit.point, -50f, 0.2f);

                Vector3 Temp = this.transform.position;
                Temp.y = 50.5f;
                Transform Position = this.transform;
                Position.position = Temp;
                SettingParticle(hit.point, Position, WCastingEffect, WTargettingEffect, 0f);

                StartCoroutine(Teleport(hit.point));
                anim.SetTrigger("QSkillTrigger");
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
    public override void Active_E_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ResetForward(hit.point, 35f, 0.2f);
                SettingParticle(hit.point, CastingPosition, ECastingEffect, ETargettingEffect, 1f);
                anim.SetTrigger("ESkillTrigger");
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

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ResetForward(hit.point, 25f, 0.2f);
                SettingParticle(hit.point, CastingPosition, RCastingEffect, RTargettingEffect, 2.8f);
                Vector3 Temp = this.transform.position;
                Temp.y = 50.5f;
                Transform Position = this.transform;
                Position.position = Temp;
                SettingParticle(hit.point,Position,RCastingEffect2,null,2.8f);
                anim.SetTrigger("RSkillTrigger");

                StartCoroutine(RSkillDelay(4f));
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
