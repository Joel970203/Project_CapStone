using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Random = UnityEngine.Random;
using Photon.Pun;

public class Boss_Skill2 : MonoBehaviourPunCallbacks
{
    private readonly object phaseLock = new object();
    PhotonView PV;
    private int currentPhase;
    private int previousPhase = 1;
    private int currentHP;
    [SerializeField] private int previousHP = 1000;

    [SerializeField] private Transform LeftFinger_Pos;
    [SerializeField] private Transform RightHand_Pos;
    [SerializeField] private Transform LeftHand_Pos;

    [SerializeField] private float P1S1_Cooltime; //Phase(P) 1의 Skill(S) 1이란 뜻
    [SerializeField] private float P1S2_Cooltime; //Phase 1의 Skill 2이란 뜻
    [SerializeField] private float P1S3_Cooltime;
    [SerializeField] private float P2S1_Cooltime;
    [SerializeField] private float P2S2_Cooltime;
    [SerializeField] private float P2S3_Cooltime;
    [SerializeField] private float P3S1_Cooltime;
    [SerializeField] private float P3S2_Cooltime;
    [SerializeField] private float P4S1_Cooltime;
    [SerializeField] private float P4S2_Cooltime;

    private float P1S1_StartTime = -1f;
    private float P1S2_StartTime = -1f;
    private float P1S3_StartTime = -1f;
    private float P2S1_StartTime = -1f;
    private float P2S2_StartTime = -1f;
    private float P2S3_StartTime = -1f;
    private float P3S1_StartTime = -1f;
    private float P3S2_StartTime = -1f;
    private float P3S3_StartTime = -1f;

    [SerializeField] private GameObject P1S2_CastingEffect;
    [SerializeField] private GameObject P2S1_CastingEffect;
    [SerializeField] private GameObject P2S2_CastingEffect;
    [SerializeField] private GameObject P3S1_CastingEffect;
    [SerializeField] private GameObject P3S2_CastingEffect;
    [SerializeField] private GameObject P4S1_CastingEffect;
    [SerializeField] private GameObject P4S2_CastingEffect;

    [SerializeField] private GameObject P1S3_Effect;


    [SerializeField] private GameObject P1S2_Bullet;
    [SerializeField] private GameObject P2S2_Bullet;

    [SerializeField] private GameObject P1S1_TargettingEffect;
    [SerializeField] private GameObject P2S1_TargettingEffect;
    [SerializeField] private GameObject P2S3_TargettingEffect;
    [SerializeField] private GameObject P3S1_TargettingEffect;
    [SerializeField] private GameObject P3S2_TargettingEffect;
    [SerializeField] private GameObject P4S1_TargettingEffect;
    [SerializeField] private GameObject P4S2_TargettingEffect;

    GameObject skillObjects;

    [SerializeField] Transform model;
    [SerializeField] Transform floor;
    [SerializeField] GameObject sword;
    [SerializeField] Transform swordVfx;
    [SerializeField] ParticleSystem swordParticle;
    [SerializeField] Transform lava;

    Material modelMaterial;
    Material floorMaterial;
    Material swordMaterial;
    Material lavaMaterial;

    private bool isP2P3 = false;
    private bool isP3P3 = false;

    Color swordColor = Color.white;

    private Animator anim;
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;
    const float tau = Mathf.PI * 2;

    private List<GameObject> targets = new List<GameObject>();
    [SerializeField] protected Transform currentTarget;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float chaseRange; //해당 범위보다 가까이 오면 추격을 시작한다.
    [SerializeField] private float chaseDuration; //추격 지속 시간, 해당 초만큼 추격을 함
    float chaseStartTime; //추격을 시작한 시간을 기록

    float distanceToTarget = Mathf.Infinity;

    bool isFinished = true; // 데드락 방지
    bool isCoroutineFinished = true;
    bool isChaseTimerSet = false; //변화여부

    bool isRotate = false;
    float rotateAngle;


    private enum State
    {
        Idle,
        Chase,
        Attack
    }
    
    State state = State.Idle;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        UpdatePlayerList();
        modelMaterial = model.GetComponent<Renderer>().material;
        floorMaterial = floor.GetComponent<Renderer>().material;
        swordMaterial = sword.GetComponent<Renderer>().material;
        lavaMaterial = lava.GetComponent<Renderer>().material;

        lineRenderer = GetComponent<LineRenderer>();
        targets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        swordVfx.gameObject.SetActive(false);
        skillObjects = new GameObject("SkillObjects");
        skillObjects.transform.SetParent(transform);
        //StartCoroutine(CheckPhase2());
    }
    // Update is called once per frame
    void Update()
    {
        //DrawRange(this.transform.position.x, this.transform.position.z, chaseRange, 50);
        //Debug.Log(state);
        UpdatePlayerList();
        currentPhase = GetComponent<Boss_Info>().GetPhaseNum();
        if (previousPhase != currentPhase && previousPhase != 4)
        {
            StopAllCoroutines();
            ChangePhase();
        }
        previousPhase = currentPhase;

        if (currentTarget != null)
        {
            transform.LookAt(currentTarget);
            if (isRotate) RotateBoss(rotateAngle);
            if (state != State.Attack) agent.SetDestination(currentTarget.position); //어택이면 stoppingDistance 라 더 가까이 갈 필요 없음
        }

        if (!isFinished || !isCoroutineFinished) return;

        isFinished = false;

        switch (state)
        {
            case State.Idle:
                JudgeStateInIdle(); //플레이어들이 접근 했는 지를 판별하고 접근 시 가장 가까운 플레이어를 추격하도록 함
                break;
            case State.Chase:
                JudgeStateInChase();
                break;
            case State.Attack:
                JudgeStateInAttack();
                break;
        }

        isFinished = true;
    }

    void UpdatePlayerList()
    {
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");

        // 현재 목록을 비우고 새로운 목록으로 채움
        targets.Clear();
        targets.AddRange(foundPlayers);
    }
    void DrawRange(float x, float z, float radius, int vertexs)
    {
        //ChangeLineColor();
        lineRenderer.positionCount = vertexs + 1;

        for (int i = 0; i <= vertexs; i++)
        {
            float currentProgress = (float)i / vertexs;

            float currentRadian = currentProgress * tau;

            float pos_X = x + radius * Mathf.Sin(currentRadian);
            float pos_Z = z + radius * Mathf.Cos(currentRadian);

            Vector3 pos = new Vector3(pos_X, 51f, pos_Z);
            lineRenderer.SetPosition(i, pos);
        }
    }

    //페이즈 변경 관련 함수

    void ChangePhase()
    {
        StartCoroutine(PhaseDelay());
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();
        currentTarget = null;
        state = State.Idle;

        switch (currentPhase)
        {
            case 2:
                anim.SetTrigger("Phase2");
                ChangeShaderP2(new Color(0.3f, 0.65f, 0.7f));
                ChangeLava();
                return;
            case 3:
                anim.SetTrigger("Phase3");
                return;
        }
    }

    //페이즈 2에 대한 변경 함수
    void ChangeShaderP2(Color swordColor)
    {
        var col = swordParticle.colorOverLifetime;
        Gradient gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { new(swordColor, 0.0f), new(swordColor, 1.0f) }
                        , new GradientAlphaKey[] { new(1.0f, 0.0f), new(0.0f, 1.0f) });
        col.color = gradient;

        sword.GetComponent<Renderer>().material.color = swordColor;
        StartCoroutine(blendTex(0.72f, 0.24f, modelMaterial)); //for boss
        StartCoroutine(blendTex(0.6f, 0.2f, floorMaterial)); //for floor
    }

    //텍스트 혼합
    IEnumerator blendTex(float endBlend, float speed, Material mat)
    {
        float startBlend = 0.0f;
        while (startBlend <= endBlend)
        {
            startBlend += speed * Time.deltaTime;
            mat.SetFloat("_Blend", startBlend);
            yield return null;
        }
    }

    IEnumerator PhaseDelay()
    {
        isCoroutineFinished = false;
        yield return new WaitForSeconds(5.0f);
        isCoroutineFinished = true;
    }

    //용암(물) 변경
    void ChangeLava()
    {
        StartCoroutine(waveColor());
        StartCoroutine(baseColor());
    }

    //물결 색상 변경
    IEnumerator waveColor()
    {
        Vector4 waveColor = new Vector4(0.3f, 0.55f, 0.75f, 0.0f);
        Vector4 startColor = new Vector4(1.6f, 0.33f, 0.18f, 0.0f);
        while (startColor.x >= waveColor.x)
        {
            startColor.x -= 0.3f * Time.deltaTime;
            if (startColor.y <= waveColor.y) startColor.y += 0.1f * Time.deltaTime;
            if (startColor.z <= waveColor.z) startColor.z += 0.3f * Time.deltaTime;
            lavaMaterial.SetVector("_WaveColor", startColor);
            yield return null;
        }
    }

    //베이스 색상 변경
    IEnumerator baseColor()
    {
        Vector4 baseColor = new Vector4(0.0f, 0.3f, 1.0f, 0.0f);
        Vector4 startColor = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        while (startColor.z <= baseColor.z)
        {
            startColor.z += 0.2f * Time.deltaTime;
            if (startColor.y <= baseColor.y) startColor.y += 0.1f * Time.deltaTime;
            lavaMaterial.SetVector("_BaseColor", startColor);
            yield return null;
        }
    }

    //페이즈 변경함수 끝

    void JudgeStateInIdle()
    {
        //if (isChaseTimerSet) SetChase(currentTarget);
        Transform nearTarget = FindNearTarget(); //가장 가까운 플레이어를 탐색
        Transform farTarget = FindFarTarget(); //가장 멀리있는 플레이어를 탐색

        if (nearTarget != null)
        {
            if (distanceToTarget >= agent.stoppingDistance) SetChase(nearTarget); //공격 범위 밖이면 추격 상태로
            else SetAttack(nearTarget); //안이면 공격 상태로
        }
        //else Debug.Log("범위 내에 플레이어가 없음");

        currentHP = this.GetComponent<Boss_Info>().HP;
        if (previousHP != currentHP) SetChase(farTarget);
    }

    void JudgeStateInChase()
    {
        Transform nearTarget = FindNearTarget();

        if (nearTarget != null)
        {
            if (nearTarget == currentTarget)
            {
                //switch (Random.Range(1, 3)) { case 1: RangedAttack(); return; } // 1/3 확률로 멀리있는 적에게 원거리 공격

                if (distanceToTarget < agent.stoppingDistance) SetAttack(currentTarget); //만일 공격 범위 내에 있으면 타겟을 공격하도록 
                else return; //공격 범위 밖이면 계속 추격하도록
            }
            else
            {
                if (distanceToTarget < agent.stoppingDistance) SetAttack(nearTarget); //대상이 바껴도 공격 범위 내에 있으면 타겟을 공격하도록 
                else //만일 현재 타겟이랑 일치하지 않으면 바꿔줌
                {
                    currentTarget = nearTarget;
                    agent.SetDestination(currentTarget.position);
                }
            }
        }
        else
        {
            if (Time.time - chaseStartTime >= chaseDuration) //만일 범위내에 플레이어가 없고 추격시간도 끝났다면 Idle 상태로
            {
                isChaseTimerSet = false;
                SetIdle();
                return;
            }
            else return; //추격 시간이 남아있다면 계속 추격한다.
        }
    }

    void JudgeStateInAttack()
    {
        Transform nearTarget = FindNearTarget();

        if (nearTarget != null)
        {
            if (distanceToTarget >= agent.stoppingDistance) //공격 범위 밖이라면 쫓아가도록
            {
                SetChase(nearTarget);
            }
            else //공격 범위 안이면 공격
            {
                SetAttack(nearTarget);
            }
        }
        else
        {   //idle->attack 이던 chase->attack 이던 currentTarget이 존재한채로 오게 됨 nearTarget이 없으면 따라가게 끝 타이머를 키고 chase로 보냄
            chaseStartTime = Time.time;
            isChaseTimerSet = true;
            SetChase(currentTarget);

            return;
        }
    }

    void SetIdle()
    {
        if (state != State.Idle) state = State.Idle;
        anim.SetTrigger("Idle");

        agent.isStopped = true; //agent를 멈추고 경로 재설정
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        currentTarget = null;
        previousHP = this.GetComponent<Boss_Info>().HP;
    }

    void SetChase(Transform nearTarget)
    {
        state = State.Chase; //상태를 추격(Chase) 상태로 바꿈
        anim.SetTrigger("Chase");

        if (!isChaseTimerSet) isChaseTimerSet = true;
        chaseStartTime = Time.time;

        currentTarget = nearTarget;
        agent.SetDestination(currentTarget.position);
        if (agent.isStopped) agent.isStopped = false;
    }

    
    void SetAttack(Transform nearTarget)
    {
        state = State.Attack;

        if (currentTarget != nearTarget) currentTarget = nearTarget;
        switch (currentPhase)
        {
            case 1:
                if(PV.IsMine && PhotonNetwork.IsConnected)
                {
                    PV.RPC("Phase1_Attack",RpcTarget.MasterClient);
                }
                return;
            case 2:
                if(PV.IsMine && PhotonNetwork.IsConnected)
                {
                    PV.RPC("Phase2_Attack",RpcTarget.MasterClient);
                }
                return;
        }
    }

    void RangedAttack()
    {
        switch (currentPhase)
        {
            case 1:
                StartCoroutine(UseSkillP1S2());
                return;
            case 2:
                StartCoroutine(UseSkillP2S2());
                return;
        }
    }

    [PunRPC]
    void Phase1_Attack(PhotonMessageInfo info)
    {
        //BaseAttack();
        //StartCoroutine(UseSkillP1S2());
        if(PhotonNetwork.IsMasterClient)
        {
            switch (Random.Range(1, 6))
            {
                case 1:
                    //Debug.Log(Time.time - P1S1_StartTime + " " + P1S1_Cooltime);
                    if (Time.time - P1S1_StartTime >= P1S1_Cooltime || P1S1_StartTime == -1f) UseSkillP1S1RPC();
                    else BaseAttackRPC();
                    return;
                case 2:
                    //Debug.Log(Time.time - P1S2_StartTime + " " + P1S2_Cooltime);
                    if (Time.time - P1S2_StartTime >= P1S2_Cooltime || P1S2_StartTime == -1f) UseSkillP1S2RPC();
                    else BaseAttackRPC();
                    return;
                case 3:
                    //Debug.Log(Time.time - P1S3_StartTime + " " + P1S3_Cooltime);
                    if (Time.time - P1S3_StartTime >= P1S3_Cooltime || P1S3_StartTime == -1f) UseSkillP1S3RPC();
                    else BaseAttackRPC();
                    return;
                default:
                    BaseAttackRPC();
                    return;
            }
        }
    }

    [PunRPC]
    void Phase2_Attack()
    {
        //StartCoroutine(UseSkillP2S2());
        if(PhotonNetwork.IsMasterClient)
        {
            switch (Random.Range(1, 6))
            {
                case 1:
                case 2:
                    if (Time.time - P2S1_StartTime >= P2S1_Cooltime || P2S1_StartTime == -1f) UseSkillP2S1RPC();
                    else BaseAttackRPC();
                    return;
                case 3:
                    if (Time.time - P2S2_StartTime >= P2S2_Cooltime || P2S2_StartTime == -1f)
                    {
                        if (!isP2P3) UseSkillP2S2RPC();
                        else BaseAttackRPC();
                    }
                    else BaseAttackRPC();
                    return;
                case 4:
                    if (Time.time - P2S3_StartTime >= P2S3_Cooltime || P2S3_StartTime == -1f) UseSkillP2S3RPC();
                    else BaseAttackRPC();
                    return;
                default:
                    BaseAttackRPC();
                    return;
            } 
        }
    }

/*
    void Phase3_Attack()
    {
        //BaseAttack();
        //StartCoroutine(UseSkillP3S4());
        
        switch (Random.Range(1, 7))
        {
            case 1:
                if (Time.time - P3S1_StartTime >= P3S1_Cooltime || P3S1_StartTime == -1f) StartCoroutine(UseSkillP3S1());
                else BaseAttack();
                return;
            case 2:
                if (Time.time - P3S2_StartTime >= P3S2_Cooltime || P3S2_StartTime == -1f) 
                {
                    if (!isP3P3) StartCoroutine(UseSkillP3S2());
                    else BaseAttack();
                }
                else BaseAttack();
                return;
            case 3:
                if (Time.time - P3S3_StartTime >= P3S3_Cooltime || P3S3_StartTime == -1f) StartCoroutine(UseSkillP3S3());
                else BaseAttack();
                return;
            case 4:
            case 5:
                if (Time.time - P3S4_StartTime >= P3S4_Cooltime || P3S4_StartTime == -1f) StartCoroutine(UseSkillP3S4());
                else BaseAttack();
                return;
            default:
                BaseAttack();
                return;
        }
        
    }
*/
    Transform FindNearTarget()
    {
        Transform nearTarget = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            distanceToTarget = Vector3.Distance(this.transform.position, target.transform.position);
            if (distanceToTarget < chaseRange)
            {
                if (distanceToTarget < minDistance)
                {
                    minDistance = distanceToTarget;
                    nearTarget = target.transform;
                }
            }
        }

        if (nearTarget != null)
        {
            swordColor = Color.red;
            distanceToTarget = minDistance;
            //Debug.Log("nearTarget: " + nearTarget.name);
        }
        else swordColor = Color.white;

        return nearTarget;
    }

    Transform FindFarTarget()
    {
        Transform farTarget = null;
        float distance;
        float maxDistance = float.Epsilon;

        foreach (GameObject target in targets)
        {
            distance = Vector3.Distance(this.transform.position, target.transform.position);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farTarget = target.transform;
            }
        }

        return farTarget;
    }
    void BaseAttackRPC()
    {
        BaseAttack();
        if(PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifyBaseAttack",RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifyBaseAttack()
    {
        PV.RPC("NotifyB",RpcTarget.All);
    }

    [PunRPC]
    void NotifyB()
    {
        BaseAttack();
    }

    void BaseAttack()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        swordVfx.gameObject.SetActive(true);

        isCoroutineFinished = false;
        //RotateBoss(60f);
        isRotate = true;
        rotateAngle = 60f;

        anim.SetTrigger("BaseAttack");
        StartCoroutine(EndBaseAttack());
    }

    private IEnumerator EndBaseAttack()
    {
        yield return new WaitForSeconds(1.65f);

        swordVfx.gameObject.SetActive(false);
        anim.SetTrigger("Rest");

        if (!isChaseTimerSet) isChaseTimerSet = true;
        chaseStartTime = Time.time;

        isRotate = false;
        isCoroutineFinished = true;
    }

    void RotateBoss(float RotateDegree)
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y += RotateDegree;
        transform.rotation = Quaternion.Euler(currentRotation);
    }

    void MakeEffectOnBoss(GameObject castingEffect, Transform castingPosition)
    {
        GameObject castEffect = Instantiate(castingEffect, castingPosition.position, castingPosition.transform.rotation);
        castEffect.transform.SetParent(castingPosition);
    }

    void MakeBulletAndShotLinear(GameObject bullet, Transform bulletPosition, Transform targetPosition, float bulletPower) //직선으로 발사
    {
        //Make Bullet
        GameObject bulletInstant = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation); //보스의 몸에서 발사
        Rigidbody bulletRigid = bulletInstant.GetComponent<Rigidbody>();

        //Shot Bullet
        Vector3 targetPos = targetPosition.position;
        //targetPos.y += 30;
        Vector3 dirVector = (targetPos - bulletPosition.position).normalized;

        bulletInstant.transform.rotation = Quaternion.LookRotation(dirVector);
        bulletRigid.AddForce(bulletInstant.transform.forward * bulletPower, ForceMode.Impulse);
        //bulletRigid.AddForce(dirVector * bulletPower, ForceMode.Impulse); (이전코드)
    }

    void MakeBulletAndShotNav(GameObject bullet, Transform bulletPosition, Transform targetPosition) //Nav를 이용해 발사
    {
        GameObject bulletInstant = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation); //보스의 몸에서 발사
        Transform target = targetPosition;
        StartCoroutine(UpdateTarget(bulletInstant, target));
    }

    IEnumerator UpdateTarget(GameObject bullet, Transform target)
    {
        NavMeshAgent nav = bullet.GetComponent<NavMeshAgent>();
        while (bullet != null)
        {
            nav.SetDestination(target.position);
            yield return null;
        }
    }

    void MakeEffectOnTarget(GameObject targetEffect, Transform targetPosition, float targetPointY)
    {
        Vector3 targetPoint = targetPosition.position;
        //if(fixY)   bool fixY, float targetPointY, bool destroy, float destroyTime)
        targetPoint.y = targetPointY;

        GameObject targetPointEffect = Instantiate(targetEffect, targetPoint, targetEffect.transform.rotation);
        targetPointEffect.transform.SetParent(skillObjects.transform);
    }

    private void EndSkill()
    {
        anim.SetTrigger("Rest");
        //anim.SetTrigger("WalkStop");
        //isAttacking = false;

        if (!isChaseTimerSet) isChaseTimerSet = true;
        chaseStartTime = Time.time;

        if (agent.isStopped) agent.isStopped = false;
        isCoroutineFinished = true;
    }




    void UseSkillP1S1RPC()
    {
        StartCoroutine(UseSkillP1S1());
        if(PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP1S1",RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP1S1()
    {
        PV.RPC("NotifyP1S1",RpcTarget.All);
    }

    [PunRPC]
    void NotifyP1S1()
    {
        StartCoroutine(UseSkillP1S1());
    }
    IEnumerator UseSkillP1S1()
    {
        isCoroutineFinished = false;
        agent.isStopped = true;
        P1S1_StartTime = Time.time;

        anim.SetTrigger("P1S1");

        float castDelay = 1.5f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) castDelay = 1.3f;
        //if (anim.GetCurrentAnimatorStateInfo(0).IsName("P1S1")) castDelay = 1.7f;

        yield return new WaitForSeconds(castDelay);
        MakeEffectOnTarget(P1S1_TargettingEffect, this.transform, 52.5f);

        yield return new WaitForSeconds(1.0f);
        EndSkill();
        //StartCoroutine(MakeEffect(null, null, null, 0f, transform, P1S1_TargettingEffect, 1.7f, 0f, 52.5f, 2.0f));
    }

    void UseSkillP1S2RPC()
    {
        StartCoroutine(UseSkillP1S2());
        if(PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP1S2",RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP1S2()
    {
        PV.RPC("NotifyP1S2",RpcTarget.All);
    }

    [PunRPC]
    void NotifyP1S2()
    {
        StartCoroutine(UseSkillP1S2());
    }
    IEnumerator UseSkillP1S2()
    {
        isCoroutineFinished = false;
        P1S2_StartTime = Time.time;

        anim.SetTrigger("P1S2");

        float animDelay = 0.3f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) animDelay = 0.1f;
        //if (anim.GetCurrentAnimatorStateInfo(0).IsName("P1S2")) animDelay = 0.3f;

        yield return new WaitForSeconds(animDelay);

        MakeEffectOnBoss(P1S2_CastingEffect, LeftFinger_Pos);

        yield return new WaitForSeconds(1.2f);

        MakeBulletAndShotLinear(P1S2_Bullet, LeftFinger_Pos, currentTarget.transform, 300.0f);

        yield return new WaitForSeconds(0.8f);
        EndSkill();
        //StartCoroutine(MakeEffect(LeftFinger_Pos, P1S2_CastingEffect, P1S2_Bullet, 300.0f, currentTarget.transform, null, 1.5f, 1.0f, 0f, 1.0f));
    }

    void UseSkillP1S3RPC()
    {
        StartCoroutine(UseSkillP1S3());
        if(PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP1S3",RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP1S3()
    {
        PV.RPC("NotifyP1S3",RpcTarget.All);
    }

    [PunRPC]
    void NotifyP1S3()
    {
        StartCoroutine(UseSkillP1S3());
    }
    IEnumerator UseSkillP1S3()
    {
        isCoroutineFinished = false;
        P1S3_StartTime = Time.time;

        float castDelay = 0.3f;
        //if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) castDelay = 1.0f;
        //else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Chase")) castDelay = 1.0f;

        anim.SetTrigger("P1S3");
        //anim.SetTrigger("Walk");

        yield return new WaitForSeconds(castDelay);
        MakeEffectOnBoss(P1S3_Effect, LeftHand_Pos);
        MakeEffectOnBoss(P1S3_Effect, RightHand_Pos);
        //isWalkingSkill = true;

        yield return new WaitForSeconds(2.5f);
        EndSkill();
        //isWalkingSkill = false;
        //StartCoroutine(MakeEffect(null, null, null, 0f, transform, P1S1_TargettingEffect, 1.7f, 0f, 52.5f, 2.0f));
    }

    void UseSkillP2S1RPC()
    {
        StartCoroutine(UseSkillP2S1());
        if(PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP2S1",RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP2S1()
    {
        PV.RPC("NotifyP2S1",RpcTarget.All);
    }

    [PunRPC]
    void NotifyP2S1()
    {
        StartCoroutine(UseSkillP2S1());
    }
    IEnumerator UseSkillP2S1()
    {
        swordVfx.gameObject.SetActive(true);
        isCoroutineFinished = false;
        P2S1_StartTime = Time.time;

        float animDelay = 1.4f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle2")) animDelay = 1.2f;

        anim.SetTrigger("P2S1");
        yield return new WaitForSeconds(animDelay);
        swordVfx.gameObject.SetActive(false);
        EndSkill();
    }

    void UseSkillP2S2RPC()
    {
        StartCoroutine(UseSkillP2S2());
        if(PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP2S2",RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP2S2()
    {
        PV.RPC("NotifyP2S2",RpcTarget.All);
    }

    [PunRPC]
    void NotifyP2S2()
    {
        StartCoroutine(UseSkillP2S2());
    }

    IEnumerator UseSkillP2S2()
    {
        sword.SetActive(false);
        isCoroutineFinished = false;
        P2S2_StartTime = Time.time;

        string animName = "P2S2";
        anim.SetTrigger(animName);

        float animDelay = 1.4f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle2")) animDelay = 1.2f;
        float castDelay = 4.3f;

        yield return new WaitForSeconds(animDelay);
        MakeEffectOnBoss(P2S2_CastingEffect, RightHand_Pos);

        yield return new WaitForSeconds(castDelay);
        MakeBulletAndShotNav(P2S2_Bullet, RightHand_Pos, currentTarget.transform);

        yield return new WaitForSeconds(3.0f);
        sword.SetActive(true);
        EndSkill();
        //StartCoroutine(MakeEffect(LeftFinger_Pos, P1S2_CastingEffect, P1S2_Bullet, 300.0f, currentTarget.transform, null, 1.5f, 1.0f, 0f, 1.0f));
    }

    void UseSkillP2S3RPC()
    {
        StartCoroutine(UseSkillP2S3());
        if(PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP2S3",RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP2S3()
    {
        PV.RPC("NotifyP2S3",RpcTarget.All);
    }

    [PunRPC]
    void NotifyP2S3()
    {
        StartCoroutine(UseSkillP2S3());
    }

    IEnumerator UseSkillP2S3()
    {
        isCoroutineFinished = false;
        P2S3_StartTime = Time.time;
        isP2P3 = true;

        float castDelay = 1.2f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle2")) castDelay = 1.0f;
        //else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Chase")) castDelay = 1.0f;

        anim.SetTrigger("P2S3");

        yield return new WaitForSeconds(castDelay);
        MakeEffectOnTarget(P2S3_TargettingEffect, this.transform, 54f);

        yield return new WaitForSeconds(0.5f);
        EndSkill();
        yield return new WaitForSeconds(8.0f);
        isP2P3 = false;
    }

/*
    IEnumerator UseSkillP3S1()
    {
        isCoroutineFinished = false;
        P3S1_StartTime = Time.time;

        float castDelay = 0.3f;
        
        anim.SetTrigger("P3S1");

        yield return new WaitForSeconds(castDelay);
        MakeEffectOnBoss(P3S1_Effect, LeftHand_Pos);
        MakeEffectOnBoss(P3S1_Effect, RightHand_Pos);

        yield return new WaitForSeconds(2.5f);
        EndSkill();
    }

    IEnumerator UseSkillP3S2()
    {  
        isCoroutineFinished = false;
        P3S2_StartTime = Time.time;

        anim.SetTrigger("P3S2");

        float animDelay = 0.5f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle3")) animDelay = 0.7f;

        yield return new WaitForSeconds(animDelay);

        MakeEffectOnBoss(P3S2_CastingEffect, LeftFinger_Pos);

        yield return new WaitForSeconds(1.6f);

        MakeBulletAndShotLinear(P3S2_Bullet, LeftFinger_Pos, currentTarget.transform, 500.0f);
        yield return new WaitForSeconds(0.3f);
        MakeBulletAndShotLinear(P3S2_Bullet, LeftFinger_Pos, currentTarget.transform, 500.0f);
        yield return new WaitForSeconds(0.3f);
        MakeBulletAndShotLinear(P3S2_Bullet, LeftFinger_Pos, currentTarget.transform, 500.0f);
        yield return new WaitForSeconds(0.3f);
        MakeBulletAndShotLinear(P3S2_Bullet, LeftFinger_Pos, currentTarget.transform, 500.0f);

        yield return new WaitForSeconds(0.8f);
        EndSkill();
    }

    IEnumerator UseSkillP3S3()
    {
        isCoroutineFinished = false;
        P3S3_StartTime = Time.time;

        float castDelay = 1.2f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle3")) castDelay = 1.0f;
        //else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Chase")) castDelay = 1.0f;

        anim.SetTrigger("P3S3");

        yield return new WaitForSeconds(castDelay);
        MakeEffectOnTarget(P3S3_TargettingEffect, this.transform, 54f);

        yield return new WaitForSeconds(0.5f);
        EndSkill();
    }

    IEnumerator UseSkillP3S4()
    {
        swordVfx.gameObject.SetActive(true);
        isCoroutineFinished = false;
        P2S1_StartTime = Time.time;

        float animDelay = 1.4f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle3")) animDelay = 1.2f;

        anim.SetTrigger("P3S4");
        yield return new WaitForSeconds(animDelay);
        swordVfx.gameObject.SetActive(false);
        EndSkill();
    }
    */
}

