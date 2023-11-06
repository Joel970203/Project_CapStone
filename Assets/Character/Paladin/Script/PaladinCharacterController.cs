using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinCharacterController : Character_Skill
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
                ResetForward(hit.point, 60f, 0.2f);

                
                anim.SetTrigger("QSkillTrigger");
                StartCoroutine(QSkillRush(this.transform.position+(hit.point-this.transform.position).normalized*50f,0.1f,0.4f));
                StartCoroutine(QSkillDelay(0.8f));
                
            }

        }
    }

    IEnumerator QSkillDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetTrigger("QSkillTrigger");
        agent.updateRotation=true;
        QCastingEffect.SetActive(false);
        yield break;
    }

    IEnumerator QSkillRush(Vector3 Point, float delay, float Time)
    {
        yield return new WaitForSeconds(delay);

        agent.updateRotation=false;
        
        agent.SetDestination(Point);
        QCastingEffect.SetActive(true);
        QCastingEffect.transform.GetChild(0).transform.Find("AttackRange").GetComponent<Collider>().enabled=true;
        yield return new WaitForSeconds(Time);
        agent.ResetPath();
        
        yield break;
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
                    Instantiate(WTargettingEffect,this.transform.position,Quaternion.identity);
                    anim.SetTrigger("WSkillTrigger");
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
                SettingParticle(hit.point, Feet, ECastingEffect, null, 0f);
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
