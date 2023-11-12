using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

public class Character_Skill : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float Q_Cooltime;

    [SerializeField]
    private float W_Cooltime;

    [SerializeField]
    private float E_Cooltime;

    [SerializeField]
    private float R_Cooltime;

    [HideInInspector]
    public float Q_Cooltime_Check;
    [HideInInspector]
    public float W_Cooltime_Check;
    [HideInInspector]
    public float E_Cooltime_Check;
    [HideInInspector]
    public float R_Cooltime_Check;

    public bool Q_Skill, W_Skill, E_Skill, R_Skill, Base_Attack;

    protected Animator anim;
    protected NavMeshAgent agent;
    void Start()
    {
        Q_Skill = true;
        W_Skill = true;
        E_Skill = true;
        R_Skill = true;
        Base_Attack = true;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        Skill_Cooltime_Cal();

        if (Input.GetKeyDown(KeyCode.Q) && Q_Skill)
        {
            Active_Q_Skill();
        }

        if (Input.GetKeyDown(KeyCode.W) && W_Skill)
        {
            Active_W_Skill();
        }

        if (Input.GetKeyDown(KeyCode.E) && E_Skill)
        {
            Active_E_Skill();
        }

        if (Input.GetKeyDown(KeyCode.R) && R_Skill)
        {
            Active_R_Skill();
        }

        if (Input.GetMouseButtonDown(0) && Base_Attack)
        {
            Active_Base_Attack();
        }
    }

    public virtual void Active_Q_Skill() { }
    public virtual void Active_W_Skill() { }
    public virtual void Active_E_Skill() { }
    public virtual void Active_R_Skill() { }

    public virtual void Active_Base_Attack() {}

    private void Skill_Cooltime_Cal()
    {
        if (Q_Cooltime_Check >= 0)
        {
            Q_Cooltime_Check -= Time.deltaTime;
            if (Q_Cooltime_Check <= 0)
            {
                Q_Cooltime_Check = 0;
                Q_Skill = true;
            }
        }

        if (W_Cooltime_Check >= 0)
        {
            W_Cooltime_Check -= Time.deltaTime;
            if (W_Cooltime_Check <= 0)
            {
                W_Cooltime_Check = 0;
                W_Skill = true;
            }
        }

        if (E_Cooltime_Check >= 0)
        {
            E_Cooltime_Check -= Time.deltaTime;
            if (E_Cooltime_Check <= 0)
            {
                E_Cooltime_Check = 0;
                E_Skill = true;
            }
        }

        if (R_Cooltime_Check >= 0)
        {
            R_Cooltime_Check -= Time.deltaTime;
            if (R_Cooltime_Check <= 0)
            {
                R_Cooltime_Check = 0;
                R_Skill = true;
            }
        }
    }

    protected void ResetForward(Vector3 HitPoint, float RotateDegree, float delay)
    {
        StartCoroutine(ResetModelForward(HitPoint, RotateDegree, delay));
    }

    //코루틴 쓰는 이유: navmesh의 resetPath가 바로 멈추지 않는 현상 발생. 캐릭터가 시전 중 바라보는 방향을 타격 지점과 일치 시키기 위해
    //                  실험을 통해 0.2초 후 바라보게 하면 의도한대로 작동한다는 것을 확인. 딜레이와 매개변수 필요하기에 코루틴 사용.
    IEnumerator ResetModelForward(Vector3 HitPoint, float RotateDegree, float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 rotationOffset = HitPoint - this.gameObject.transform.position;
        rotationOffset.y = 0;
        this.gameObject.transform.forward = rotationOffset;

        //Y축(세로축)을 -50도 회전. (애니메이션 자체에 회전이 들어가서 타격 지점을 바라봐도 축이 틀어짐.)
        Vector3 currentRotation = this.gameObject.transform.rotation.eulerAngles;
        currentRotation.y += RotateDegree;
        this.gameObject.transform.rotation = Quaternion.Euler(currentRotation);
        yield break;
    }

    protected void SettingParticle(Vector3 HitPoint, Transform CastingPosition, GameObject CastingEffect, GameObject TargetEffect, float delay)
    {
        StartCoroutine(MakeEffect(HitPoint, CastingPosition, CastingEffect, TargetEffect, delay));
    }
    IEnumerator MakeEffect(Vector3 HitPoint, Transform CastingPosition, GameObject CastingEffect, GameObject TargetEffect, float delay)
    {
        if (CastingEffect != null)
        {
            GameObject CastingEffet = Instantiate(CastingEffect, CastingPosition.position, CastingEffect.transform.rotation);
            CastingEffet.transform.parent = CastingPosition;
        }


        yield return new WaitForSeconds(delay);



        if (TargetEffect != null)
        {
            Vector3 TargetPoint = HitPoint;
            TargetPoint.y = 50.5f;

            GameObject TargetPointEffect = Instantiate(TargetEffect, TargetPoint, TargetEffect.transform.rotation);
        }

        yield break;
    }
}
