using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Photon.Pun;
public class MageCharacterSkill : MonoBehaviourPunCallbacks
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
            NormalAttack();
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
       Q_Cooltime_Check = Q_Cooltime;
       W_Cooltime_Check = W_Cooltime;
       E_Cooltime_Check = E_Cooltime;
       R_Cooltime_Check = R_Cooltime;
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
    [PunRPC]
    protected void StopMove(float delay)
    {
        StartCoroutine(StopClickMove(delay));
    }

    IEnumerator StopClickMove(float delay)
    {
        this.gameObject.GetComponent<Mage_Move>().AllStop=true;
        yield return new WaitForSeconds(delay);
        this.gameObject.GetComponent<Mage_Move>().AllStop=false;
        yield break;
    }



    //기본 공격
    //화면상 마우스 위치로 레이캐스트를 쏴서 타격 지점을 구하고 현재 캐릭터 포지션과 타격 지점의 방향을 구해
    //해당 방향으로 생성된 오브젝트(에너지 볼트)가 이동하게 구현.
    public void NormalAttack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << LayerMask.NameToLayer("Moveable");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                PV.RPC("callNormalAttack", RpcTarget.AllViaServer, hit.point);
            }
        }
    }

    [PunRPC]
    void callNormalAttack(Vector3 targetPosition)
    {
        StopMove(1f);
        anim.SetBool("Walk", false);
        agent.ResetPath();
        ResetForward(targetPosition, -50f, 0.2f);
        SettingParticle(targetPosition, CastingPosition, QCastingEffect, null, 0.2f);
        StartCoroutine(ShootEnergyBolt(targetPosition));
        anim.SetTrigger("QSkillTrigger");
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

    public void Active_Q_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int layerMask = 1 << LayerMask.NameToLayer("Moveable");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                
                PV.RPC("callQ", RpcTarget.AllViaServer, hit.point);
            }
        }
    }

    [PunRPC]
    void callQ(Vector3 targetPosition)
    {
        StopMove(1.5f);
        anim.SetBool("Walk", false);
        agent.ResetPath();
        ResetForward(targetPosition,-50f,0.2f);
        SettingParticle(targetPosition, CastingPosition, QCastingEffect, QTargettingEffect, 0.2f);
        anim.SetTrigger("QSkillTrigger");
        Q_Cooltime_Check = Q_Cooltime; // 1
        Q_Skill = false; // 2 

    }

    //메이지 W 스킬. 텔레포트
    //화면상 마우스 위치로 레이캐스트를 쏴서 해당 위치로 캐릭터가 이동하는 함수.
    public void Active_W_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int layerMask = 1 << LayerMask.NameToLayer("Moveable");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                PV.RPC("callW", RpcTarget.AllViaServer, hit.point);
            }
        }
    }


    [PunRPC]
    void callW(Vector3 targetPosition)
    {
        StopMove(3f);
        anim.SetBool("Walk", false);
        agent.ResetPath();
        ResetForward(targetPosition, -50f, 0.2f);

        Vector3 Temp = this.transform.position;
        Temp.y = 50.5f;
        Transform Position = this.transform;
        Position.position = Temp;
        SettingParticle(targetPosition, Position, WCastingEffect, WTargettingEffect, 0f);
        StartCoroutine(Teleport(targetPosition));
        anim.SetTrigger("QSkillTrigger");
        W_Cooltime_Check = W_Cooltime;
        W_Skill = false;       
    }
    IEnumerator Teleport(Vector3 targetPosition)
    {
        yield return new WaitForSeconds(1.2f);

        Vector3 WarpPosition = new Vector3(targetPosition.x, 60f, targetPosition.z);

        agent.Warp(WarpPosition);
        yield break;
    }
    
    //메이지 E 스킬. 지속딜 주는 불기둥 소환
    //화면상 마우스 위치로 레이캐스트를 쏴서 해당 위치에 불기둥 오브젝트 생성하는 함수.
    [PunRPC]
    public void Active_E_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int layerMask = 1 << LayerMask.NameToLayer("Moveable");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                PV.RPC("callE", RpcTarget.AllViaServer, hit.point);
            }
        }
    }


    [PunRPC]
    void callE(Vector3 targetPosition)
    {
        StopMove(3f);
        anim.SetBool("Walk", false);
        agent.ResetPath();
        ResetForward(targetPosition, 35f, 0.2f);
        SettingParticle(targetPosition, CastingPosition, ECastingEffect, ETargettingEffect, 1f);
        anim.SetTrigger("ESkillTrigger");
        E_Cooltime_Check = E_Cooltime;
        E_Skill = false;
    }


    //메이지 R스킬
    //화면상 마우스 위치에 레이캐스트를 쏴서 해당 위치를 타격 지점으로 하고, 메테오 오브젝트 소환.
    //메테오 오브젝트튼 타격 위치로 이동함. 일정 시간 후 (땅에 박혔을 때) 폭발하면서 주변에 데미지를 줌.
    [PunRPC]
    public void Active_R_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int layerMask = 1 << LayerMask.NameToLayer("Moveable");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                PV.RPC("callR", RpcTarget.AllViaServer, hit.point);   
            }
        }
    }


    [PunRPC]
    void callR(Vector3 targetPosition)
    {
        StopMove(3f);
        anim.SetBool("Walk", false);
        agent.ResetPath();
        ResetForward(targetPosition, 25f, 0.2f);
        SettingParticle(targetPosition, CastingPosition, RCastingEffect, RTargettingEffect, 2.8f);
        Vector3 Temp = this.transform.position;
        Temp.y = 50.5f;
        Transform Position = this.transform;
        Position.position = Temp;
        SettingParticle(targetPosition, Position, RCastingEffect2, null, 2.8f);
        anim.SetTrigger("RSkillTrigger");
        StartCoroutine(RSkillDelay(4f));
        R_Cooltime_Check = R_Cooltime;
        R_Skill = false;
    }

    IEnumerator RSkillDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetTrigger("RSkillTrigger");
        yield break;
    }
}
