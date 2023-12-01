using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealerCharacterSkill : MonoBehaviourPunCallbacks
{
    [SerializeField]
    protected float Q_Cooltime;

    [SerializeField]
    protected float W_Cooltime;

    [SerializeField]
    protected float E_Cooltime;

    [SerializeField]
    protected float R_Cooltime;

    public Sprite[] SkillIcons;

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
        if (PV.IsMine)
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

    protected void StopMove(float delay)
    {
        StartCoroutine(StopClickMove(delay));
    }

    IEnumerator StopClickMove(float delay)
    {
        this.gameObject.GetComponent<Healer_Move>().AllStop = true;
        yield return new WaitForSeconds(delay);
        this.gameObject.GetComponent<Healer_Move>().AllStop = false;
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
                PV.RPC("HealerNormalAttack", RpcTarget.AllViaServer, hit.point);
            }
        }
    }

    [PunRPC]
    void HealerNormalAttack(Vector3 Point)
    {
        StopMove(0.5f);
        anim.SetBool("Walk", false);
        agent.ResetPath();
        ResetForward(Point, -50f, 0.2f);
        SettingParticle(Point, CastingPosition, null, null, 0.2f);
        StartCoroutine(ShootEnergyBolt(Point));
        anim.SetTrigger("QSkillTrigger");
        StartCoroutine(QSkillDelay(0.5f));
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
    public void Active_Q_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int layerMask = 1 << LayerMask.NameToLayer("Moveable");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                PV.RPC("HealerQSkill", RpcTarget.AllViaServer, hit.point);
            }
        }
    }

    [PunRPC]
    void HealerQSkill(Vector3 Point)
    {
        StopMove(1.5f);
        anim.SetBool("Walk", false);
        agent.ResetPath();
        ResetForward(Point, -50f, 0.2f);
        SettingParticle(Point, Back, QCastingEffect, QTargettingEffect, 0.2f);
        anim.SetTrigger("QSkillTrigger");
        StartCoroutine(QSkillDelay(1.5f));
        Q_Cooltime_Check = Q_Cooltime;
        Q_Skill = false;
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
    public void Active_W_Skill()
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
                    PV.RPC("HealerWSkill", RpcTarget.AllViaServer, hit.point);
                }
            }
        }
    }

    [PunRPC]
    void HealerWSkill(Vector3 Point)
    {
        StopMove(1.0f);
        anim.SetBool("Walk", false);
        agent.ResetPath();
        ResetForward(Point, -50f, 0.2f);
        SettingParticle(Point, CastingPosition, WCastingEffect, WTargettingEffect, 0f);
        anim.SetTrigger("WSkillTrigger");
        W_Cooltime_Check = W_Cooltime;
        W_Skill = false;
    }

    //힐러 E 스킬. 광역 버프
    //화면상 마우스 위치로 레이캐스트를 쏴서 해당 위치를 캐릭터가 바라보게 하고, 애니메이션과 파티클 재생.
    public void Active_E_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int layerMask = 1 << LayerMask.NameToLayer("Moveable");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                PV.RPC("HealerESkill", RpcTarget.AllViaServer, hit.point);
            }
        }
    }

    [PunRPC]
    void HealerESkill(Vector3 Point)
    {
        StopMove(3f);
        anim.SetBool("Walk", false);
        agent.ResetPath();
        ResetForward(Point, 0f, 0.2f);
        SettingParticle(Point, Feet, ECastingEffect, null, 1f);
        anim.SetTrigger("ESkillTrigger");
        E_Cooltime_Check = E_Cooltime;
        E_Skill = false;
    }

    //힐러 R 스킬. 부활
    //화면상 마우스 위치로 레이캐스트를 쏴서 맞은 오브젝트의 레이어를 분석. Layer가 Grave라면 동작.
    //애니메이션 재생하며 Grave에 기록된 플레이어를 활성화하고, Grave 오브젝트 비활성화.
    public void Active_R_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            int layerMask = 1 << LayerMask.NameToLayer("Grave");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                PV.RPC("HealerRSkill", RpcTarget.AllViaServer, hit.point);
                StartCoroutine(TriggerRivival(hit.collider.gameObject, 4f));
            }
        }
    }
    [PunRPC]
    void HealerRSkill(Vector3 Point)
    {
        StopMove(3f);
        anim.SetBool("Walk", false);
        agent.ResetPath();
        ResetForward(Point, 60f, 0.2f);
        SettingParticle(Point, Feet, RCastingEffect, null, 2.8f);
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

    IEnumerator TriggerRivival(GameObject Grave, float delay)
    {
        yield return new WaitForSeconds(delay);
        Grave.GetComponent<RivivalCharacter>().rivival();
        yield break;
    }
}
