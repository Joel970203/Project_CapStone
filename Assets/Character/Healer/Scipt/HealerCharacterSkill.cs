using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerCharacterSkill : Character_Skill
{
    [SerializeField]
    private Transform CastingPosition;

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

                SettingParticle(hit.point, Back, QCastingEffect, QTargettingEffect, 0.2f);
                anim.SetTrigger("QSkillTrigger");
                StartCoroutine(QSkillDelay(1.5f));
            }

        }
    }

    IEnumerator QSkillDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetTrigger("QSkillTrigger");
        yield break;
    }

    public override void Active_W_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

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
                    ResetForward(hit.point, -50f, 0.2f);
                    SettingParticle(hit.point, CastingPosition, WCastingEffect, WTargettingEffect, 0f);
                    anim.SetTrigger("WSkillTrigger");
                }
            }

        }
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
                ResetForward(hit.point, 0f, 0.2f);
                SettingParticle(hit.point, Feet, ECastingEffect, null, 1f);
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

            int layerMask = 1 << LayerMask.NameToLayer("Grave");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                ResetForward(hit.point, 60f, 0.2f);
                SettingParticle(hit.point, Feet, RCastingEffect, null, 2.8f);
                anim.SetTrigger("RSkillTrigger");

                StartCoroutine(RSkillDelay(4f));

                StartCoroutine(TriggerRivival(hit.collider.gameObject,4f));
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
