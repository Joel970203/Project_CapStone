using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PaladinCharacterController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    protected float Q_Cooltime;

    [SerializeField]
    protected float W_Cooltime;

    [SerializeField]
    protected float E_Cooltime;

    [SerializeField]
    protected float R_Cooltime;

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
    protected UnityEngine.AI.NavMeshAgent agent;
    
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
    PhotonView PV;
    void Start()
    {
        Q_Skill = true;
        W_Skill = true;
        E_Skill = true;
        R_Skill = true;
        Base_Attack = true;
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

        Skill_Cooltime_Cal();

        if (Input.GetMouseButtonDown(0))
        {
            //NormalAttack();
        }

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
    }

    public void ResetCoolDown()
    {
        Q_Skill = true;
        W_Skill = true;
        E_Skill = true;
        R_Skill = true;
    }

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

        //(애니메이션 자체에 회전이 들어가서 타격 지점을 바라봐도 축이 틀어짐.)
        Vector3 currentRotation = this.gameObject.transform.rotation.eulerAngles;
        currentRotation.y += RotateDegree;
        this.gameObject.transform.rotation = Quaternion.Euler(currentRotation);
        yield break;
    }

    //이펙트 만드는 함수 SettingParticle
    //캐스팅 위치, 타겟 위치를 인자로 받고 해당 위치에 특정 시간 후에 지정한 이펙트(오브젝트)가 생성되게끔함.
    //오브젝트에 스스로 특정 시간 뒤에 destoy하는 기능이 있고, 데미지 적용도 들어가 있어서 여기서 생성만 해주면 된다.
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

    //공격 애니메이션 작동 중에 클릭 연타시 해당 위치로 이동하는 문제점 발견(의도는 가만히 서서 애니메이션 작동하길 원함)
    //이동하면서 walk 애니메이션이 재생되는 문제를 해결하기 위해 모든 이동 입력을 막는 bool 값 "AllStop"을 설정해줘서 컨트롤 하고자 함.
    protected void StopMove(float delay)
    {
        StartCoroutine(StopClickMove(delay));
    }

    IEnumerator StopClickMove(float delay)
    {
        this.gameObject.GetComponent<Paladin_Move>().AllStop=true;
        yield return new WaitForSeconds(delay);
        this.gameObject.GetComponent<Paladin_Move>().AllStop=false;
        yield break;
    }

    public void Active_Q_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
              PV.RPC("Paladin_QSkill",RpcTarget.All,hit.point);
            }

        }
    }

    [PunRPC]
    public void Paladin_QSkill(Vector3 Point)
    {
        ResetForward(Point, 60f, 0.2f);
        anim.SetTrigger("QSkillTrigger");
        StartCoroutine(QSkillRush(this.transform.position+(Point-this.transform.position).normalized*50f,0.1f,0.4f));
        StartCoroutine(QSkillDelay(0.8f));
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


    public void Active_W_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                PV.RPC("Paladin_WSkill",RpcTarget.All,hit.point);
            }

        }
    }

    [PunRPC]
    public void Paladin_WSkill(Vector3 Point)
    {
       ResetForward(Point, -50f, 0.2f);
        Instantiate(WTargettingEffect,this.transform.position,Quaternion.identity);
        anim.SetTrigger("WSkillTrigger");
    }

    public void Active_E_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                PV.RPC("Paladin_ESkill",RpcTarget.All,hit.point);
            }

        }
    }

    [PunRPC]
    public void Paladin_ESkill(Vector3 Point)
    {
       ResetForward(Point, 0f, 0.2f);
       SettingParticle(Point, Feet, ECastingEffect, null, 0f);
       anim.SetTrigger("ESkillTrigger");
    }

    public void Active_R_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

            int layerMask = 1 << LayerMask.NameToLayer("Grave");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                PV.RPC("Paladin_RSkill",RpcTarget.All,hit.point);
                StartCoroutine(TriggerRivival(hit.collider.gameObject,4f));
            }
        }

    }

    [PunRPC]
    public void Paladin_RSkill(Vector3 Point)
    {
       ResetForward(Point, 60f, 0.2f);
       SettingParticle(Point, Feet, RCastingEffect, null, 2.8f);
       anim.SetTrigger("RSkillTrigger");
       StartCoroutine(RSkillDelay(4f));
      
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

