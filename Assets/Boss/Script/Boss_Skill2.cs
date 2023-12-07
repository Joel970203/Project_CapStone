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

    [SerializeField] private GameObject warrok;
    [SerializeField] Material flatBwMat;
    private SkinnedMeshRenderer warrokMeshRenderer;

    [SerializeField] private GameObject Effect_Phase3;
    [SerializeField] private GameObject refractShpere;

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
    [SerializeField] private float P3S3_Cooltime;
    [SerializeField] private float P3S4_Cooltime;
    [SerializeField] private float P3S5_Cooltime;

    private float P1S1_Cooltime_Check;
    private float P1S2_Cooltime_Check;
    private float P1S3_Cooltime_Check;
    private float P2S1_Cooltime_Check;
    private float P2S2_Cooltime_Check;
    private float P2S3_Cooltime_Check;
    private float P3S1_Cooltime_Check;
    private float P3S2_Cooltime_Check;
    private float P3S3_Cooltime_Check;
    private float P3S4_Cooltime_Check;
    private float P3S5_Cooltime_Check;

    private bool P1S1 = true, P1S2 = true, P1S3 = true, P2S1 = true, P2S2 = true, P2S3 = true, P3S1 = true, P3S2 = true, P3S3 = true, P3S4 = true, P3S5 = true;

    [SerializeField] private GameObject P1S2_CastingEffect;
    [SerializeField] private GameObject P2S2_CastingEffect;
    [SerializeField] private GameObject P3S2_CastingEffect;

    [SerializeField] private GameObject P1S3_Effect;
    [SerializeField] private GameObject P3S1_Effect;

    [SerializeField] private GameObject P1S2_Bullet;
    [SerializeField] private GameObject P2S2_Bullet;
    [SerializeField] private GameObject P3S2_Bullet;

    [SerializeField] private GameObject P1S1_TargettingEffect;
    [SerializeField] private GameObject P2S3_TargettingEffect;
    [SerializeField] private GameObject P3S3_TargettingEffect;

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

    private Animator anim;
    private NavMeshAgent agent;

    private List<GameObject> targets = new List<GameObject>();
    [SerializeField] protected Transform currentTarget;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float chaseRange; //해당 범위보다 가까이 오면 추격을 시작한다.
    [SerializeField] private float chaseDuration; //해당 초만큼 추격을 함
    float chaseTime_Check; //추격 중인 시간을 기록
    bool isChaseTimerSet = false;

    float distanceToTarget = Mathf.Infinity;

    bool isFinished = true; // 데드락 방지
    bool isCoroutineFinished = true;

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
        warrokMeshRenderer = warrok.GetComponent<SkinnedMeshRenderer>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        UpdatePlayerList();
        modelMaterial = model.GetComponent<Renderer>().material;
        floorMaterial = floor.GetComponent<Renderer>().material;
        swordMaterial = sword.GetComponent<Renderer>().material;
        lavaMaterial = lava.GetComponent<Renderer>().material;

        targets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        swordVfx.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerList();
        currentPhase = GetComponent<Boss_Info>().GetPhaseNum();
        if (previousPhase != currentPhase && previousPhase != 4)
        {
            StopAllCoroutines();
            ChangePhase();
        }
        previousPhase = currentPhase;

        Skill_Cooltime_Cal();

        if (currentTarget != null)
        {
            transform.LookAt(currentTarget);
            if (isRotate) RotateBoss(rotateAngle);
            if (state != State.Attack) agent.SetDestination(currentTarget.position); //어택이면 stoppingDistance 라 더 가까이 갈 필요 없음
        }

        if (!isFinished || !isCoroutineFinished) return;

        isFinished = false;
        currentHP = GetComponent<Boss_Info>().HP;

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

        previousHP = GetComponent<Boss_Info>().HP;
        isFinished = true;
    }

    void UpdatePlayerList()
    {
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");

        // 현재 목록을 비우고 새로운 목록으로 채움
        targets.Clear();
        targets.AddRange(foundPlayers);
    }

    //스킬 쿨타임 및 추격 타이머 확인 확인

    void Skill_Cooltime_Cal()
    {
        if (isChaseTimerSet)
        {
            if (chaseTime_Check >= 0)
            {
                chaseTime_Check -= Time.deltaTime;

                if (chaseTime_Check <= 0)
                {
                    isChaseTimerSet = false;
                }
            }
        }

        switch (currentPhase)
        {
            case 1:
                if (P1S1_Cooltime_Check >= 0)
                {
                    P1S1_Cooltime_Check -= Time.deltaTime;

                    if (P1S1_Cooltime_Check <= 0)
                    {
                        P1S1_Cooltime_Check = 0;
                        P1S1 = true;
                    }
                }
                if (P1S2_Cooltime_Check >= 0)
                {
                    P1S2_Cooltime_Check -= Time.deltaTime;

                    if (P1S2_Cooltime_Check <= 0)
                    {
                        P1S2_Cooltime_Check = 0;
                        P1S2 = true;
                    }
                }
                if (P1S3_Cooltime_Check >= 0)
                {
                    P1S3_Cooltime_Check -= Time.deltaTime;

                    if (P1S3_Cooltime_Check <= 0)
                    {
                        P1S3_Cooltime_Check = 0;
                        P1S3 = true;
                    }
                }
                return;
            case 2:
                if (P2S1_Cooltime_Check >= 0)
                {
                    P2S1_Cooltime_Check -= Time.deltaTime;

                    if (P2S1_Cooltime_Check <= 0)
                    {
                        P2S1_Cooltime_Check = 0;
                        P2S1 = true;
                    }
                }
                if (P2S2_Cooltime_Check >= 0)
                {
                    P2S2_Cooltime_Check -= Time.deltaTime;

                    if (P2S2_Cooltime_Check <= 0)
                    {
                        P2S2_Cooltime_Check = 0;
                        P2S2 = true;
                    }
                }
                if (P2S3_Cooltime_Check >= 0)
                {
                    P2S3_Cooltime_Check -= Time.deltaTime;

                    if (P2S3_Cooltime_Check <= 0)
                    {
                        P2S3_Cooltime_Check = 0;
                        P2S3 = true;
                    }
                }
                return;
            case 3:
                if (P3S1_Cooltime_Check >= 0)
                {
                    P3S1_Cooltime_Check -= Time.deltaTime;

                    if (P3S1_Cooltime_Check <= 0)
                    {
                        P3S1_Cooltime_Check = 0;
                        P3S1 = true;
                    }
                }
                if (P3S2_Cooltime_Check >= 0)
                {
                    P3S2_Cooltime_Check -= Time.deltaTime;

                    if (P3S2_Cooltime_Check <= 0)
                    {
                        P3S2_Cooltime_Check = 0;
                        P3S2 = true;
                    }
                }
                if (P3S3_Cooltime_Check >= 0)
                {
                    P3S3_Cooltime_Check -= Time.deltaTime;

                    if (P3S3_Cooltime_Check <= 0)
                    {
                        P3S3_Cooltime_Check = 0;
                        P3S3 = true;
                    }
                }
                if (P3S4_Cooltime_Check >= 0)
                {
                    P3S4_Cooltime_Check -= Time.deltaTime;

                    if (P3S4_Cooltime_Check <= 0)
                    {
                        P3S4_Cooltime_Check = 0;
                        P3S4 = true;
                    }
                }
                if (P3S5_Cooltime_Check >= 0)
                {
                    P3S5_Cooltime_Check -= Time.deltaTime;

                    if (P3S5_Cooltime_Check <= 0)
                    {
                        P3S5_Cooltime_Check = 0;
                        P3S5 = true;
                    }
                }
                return;
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
                anim.SetTrigger("Yelling");
                ChangeShaderP2(new Color(0.3f, 0.65f, 0.7f));
                ChangeLava();
                return;
            case 3:
                anim.SetTrigger("Pain");
                StartCoroutine(ChangeShaderP3(new Color(0.2f, 0.8f, 0.1f)));
                return;
            case 4:
                anim.SetTrigger("Death");
                this.enabled = false;
                return;
        }
    }

    //페이즈 하는 동안 딜레이 주는 함수
    IEnumerator PhaseDelay()
    {
        isCoroutineFinished = false;

        switch (currentPhase)
        {
            case 2:
                yield return new WaitForSeconds(3.0f);
                anim.SetTrigger("Phase2");
                break;
            case 3:
                yield return new WaitForSeconds(0.5f);
                warrokMeshRenderer.material = flatBwMat;
                yield return new WaitForSeconds(1.7f);
                anim.SetTrigger("Yelling3");
                yield return new WaitForSeconds(2.0f);
                anim.SetTrigger("Phase3");
                refractShpere.SetActive(true);
                break;
        }

        isCoroutineFinished = true;
    }

    //페이즈 2에 대한 변경 함수
    void ChangeShaderP2(Color swordColor)
    {
        var col = swordParticle.colorOverLifetime;
        Gradient gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { new(swordColor, 0.0f), new(swordColor, 1.0f) }
                        , new GradientAlphaKey[] { new(1.0f, 0.0f), new(0.0f, 1.0f) });
        col.color = gradient;

        StartCoroutine(BlendTex(0.72f, 0.24f, modelMaterial)); //for boss
        StartCoroutine(BlendTex(0.9f, 0.3f, swordMaterial)); //for sword
        StartCoroutine(BlendTex(0.6f, 0.2f, floorMaterial)); //for floor
    }

    //페이즈 3에 대한 변경 함수
    IEnumerator ChangeShaderP3(Color swordColor)
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(Effect_Phase3, transform.position, Effect_Phase3.transform.rotation);
        var col = swordParticle.colorOverLifetime;
        Gradient gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { new(swordColor, 0.0f), new(swordColor, 1.0f) }
                        , new GradientAlphaKey[] { new(1.0f, 0.0f), new(0.0f, 1.0f) });
        col.color = gradient;

        StartCoroutine(NotBlendTex(0.72f, 0.24f, modelMaterial)); //for boss
        StartCoroutine(NotBlendTex(0.9f, 0.3f, swordMaterial)); //for sword
        StartCoroutine(NotBlendTex(0.6f, 0.2f, floorMaterial)); //for floor
    }

    //텍스처 혼합
    IEnumerator BlendTex(float endBlend, float speed, Material mat)
    {
        float startBlend = 0.0f;
        while (startBlend <= endBlend)
        {
            startBlend += speed * Time.deltaTime;
            mat.SetFloat("_Blend", startBlend);
            yield return null;
        }
    }

    //텍스처 혼합 이전 상태로
    IEnumerator NotBlendTex(float startBlend, float speed, Material mat)
    {
        float endBlend = 0.0f;
        while (startBlend >= endBlend)
        {
            startBlend -= speed * Time.deltaTime;
            mat.SetFloat("_Blend", startBlend);
            yield return null;
        }
    }

    //용암(물) 변경
    void ChangeLava()
    {
        StartCoroutine(WaveColor());
        StartCoroutine(BaseColor());
        StartCoroutine(WavePower());
    }

    //물결 색상 변경
    IEnumerator WaveColor()
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
    IEnumerator BaseColor()
    {
        Vector4 baseColor = new Vector4(0.0f, 0.3f, 0.8f, 0.0f);
        Vector4 startColor = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        while (startColor.z <= baseColor.z)
        {
            startColor.z += 0.2f * Time.deltaTime;
            if (startColor.y <= baseColor.y) startColor.y += 0.1f * Time.deltaTime;
            lavaMaterial.SetVector("_BasedColor", startColor);
            yield return null;
        }
    }

    //물결 세기 변경
    IEnumerator WavePower()
    {
        float endColor = 30;
        float startPower = 12;
        while (startPower <= endColor)
        {
            startPower += 6 * Time.deltaTime;
            lavaMaterial.SetFloat("_wavePower", startPower);
            yield return null;
        }
    }

    //페이즈 변경함수 끝

    void JudgeStateInIdle()
    {
        //if (isChaseTimerSet) SetChase(currentTarget);
        Transform nearTarget = FindNearTarget(); //가장 가까운 플레이어를 탐색

        if (nearTarget != null)
        {
            if (distanceToTarget >= agent.stoppingDistance) SetChase(nearTarget); //공격 범위 밖이면 추격 상태로
            else SetAttack(nearTarget); //안이면 공격 상태로
        }
        //else Debug.Log("범위 내에 플레이어가 없음");

        currentHP = this.GetComponent<Boss_Info>().HP;
        if (previousHP != currentHP) SetChase(FindFarTarget());
    }

    void JudgeStateInChase()
    {
        Transform nearTarget = FindNearTarget();

        if (nearTarget != null)
        {
            if (nearTarget == currentTarget)
            {
                if (Random.Range(1, 1000) == 1) RangedAttack();// 추격 중 적에게 원거리 공격

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
            if (!isChaseTimerSet) //만일 범위내에 플레이어가 없고 추격타이머도 끝났다면 Idle 상태로
            {
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
            chaseTime_Check = chaseDuration;
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
        chaseTime_Check = chaseDuration;

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
                if (PV.IsMine && PhotonNetwork.IsConnected)
                {
                    PV.RPC("Phase1_Attack", RpcTarget.MasterClient);
                }
                return;
            case 2:
                if (PV.IsMine && PhotonNetwork.IsConnected)
                {
                    PV.RPC("Phase2_Attack", RpcTarget.MasterClient);
                }
                return;
            case 3:
                if (PV.IsMine && PhotonNetwork.IsConnected)
                {
                    PV.RPC("Phase3_Attack", RpcTarget.MasterClient);
                }
                return;
        }
    }

    void RangedAttack()
    {
        switch (currentPhase)
        {
            case 1:
                if (P1S2)
                {
                    agent.isStopped = true;
                    state = State.Attack;
                    StartCoroutine(UseSkillP1S2());
                }
                else state = State.Chase;
                return;
            case 2:
                if (P2S2)
                {
                    agent.isStopped = true;
                    state = State.Attack;
                    StartCoroutine(UseSkillP2S2());
                }
                else state = State.Chase;
                return;
            case 3:
                if (P3S2)
                {
                    agent.isStopped = true;
                    state = State.Attack;
                    StartCoroutine(UseSkillP3S2());
                }
                else state = State.Chase;
                return;
        }
    }

    [PunRPC]
    void Phase1_Attack(PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            switch (Random.Range(1, 4))
            {
                case 1:
                    if (P1S1) UseSkillP1S1RPC();
                    else BaseAttackRPC();
                    return;
                case 2:
                    if (P1S2) UseSkillP1S2RPC();
                    else BaseAttackRPC();
                    return;
                case 3:
                    if (P1S3) UseSkillP1S3RPC();
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
        if (PhotonNetwork.IsMasterClient)
        {
            switch (Random.Range(1, 8))
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    if (P2S1) UseSkillP2S1RPC();
                    else BaseAttackRPC();
                    return;
                case 5:
                    if (P2S2) UseSkillP2S2RPC();
                    else BaseAttackRPC();
                    return;
                case 6:
                case 7:
                    if (P2S3) UseSkillP2S3RPC();
                    else BaseAttackRPC();
                    return;
                default:
                    BaseAttackRPC();
                    return;
            }
        }
    }

    void Phase3_Attack()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            switch (Random.Range(1, 8))
            {
                case 1:
                    if (P3S1) UseSkillP3S1RPC();
                    else BaseAttackRPC();
                    return;
                case 2:
                    if (P3S2) UseSkillP3S2RPC();
                    else BaseAttackRPC();
                    return;
                case 3:
                    if (P3S3) UseSkillP3S3RPC();
                    else BaseAttackRPC();
                    return;
                case 4:
                case 5:
                    if (P3S4) UseSkillP3S4RPC();
                    else BaseAttackRPC();
                    return;
                case 7:
                    if (P3S5) UseSkillP3S4RPC();
                    else BaseAttackRPC();
                    return;
                default:
                    BaseAttackRPC();
                    return;
            }
        }
    }

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
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifyBaseAttack", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifyBaseAttack()
    {
        PV.RPC("NotifyB", RpcTarget.All);
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
        chaseTime_Check = chaseDuration;

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
        Vector3 dirVector = (targetPos - bulletPosition.position).normalized;

        bulletInstant.transform.rotation = Quaternion.LookRotation(dirVector);
        bulletRigid.AddForce(bulletInstant.transform.forward * bulletPower, ForceMode.Impulse);
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
        targetPoint.y = targetPointY;

        GameObject targetPointEffect = Instantiate(targetEffect, targetPoint, targetEffect.transform.rotation);
    }

    private void EndSkill()
    {
        anim.SetTrigger("Rest");

        if (!isChaseTimerSet) isChaseTimerSet = true;
        chaseTime_Check = chaseDuration;

        if (agent.isStopped) agent.isStopped = false;
        isCoroutineFinished = true;
    }

    void UseSkillP1S1RPC()
    {
        StartCoroutine(UseSkillP1S1());
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP1S1", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP1S1()
    {
        PV.RPC("NotifyP1S1", RpcTarget.All);
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

        anim.SetTrigger("P1S1");

        float castDelay = 1.8f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) castDelay = 1.6f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Chase")) castDelay = 1.4f;

        yield return new WaitForSeconds(castDelay);
        MakeEffectOnTarget(P1S1_TargettingEffect, this.transform, 52.5f);

        yield return new WaitForSeconds(1.0f);
        P1S1_Cooltime_Check = P1S1_Cooltime;
        P1S1 = false;
        EndSkill();
    }

    void UseSkillP1S2RPC()
    {
        StartCoroutine(UseSkillP1S2());
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP1S2", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP1S2()
    {
        PV.RPC("NotifyP1S2", RpcTarget.All);
    }

    [PunRPC]
    void NotifyP1S2()
    {
        StartCoroutine(UseSkillP1S2());
    }
    IEnumerator UseSkillP1S2()
    {
        Transform oldTarget = currentTarget;
        currentTarget = FindFarTarget();

        isCoroutineFinished = false;

        anim.SetTrigger("P1S2");

        float animDelay = 0.3f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) animDelay = 0.1f;

        yield return new WaitForSeconds(animDelay);

        MakeEffectOnBoss(P1S2_CastingEffect, LeftFinger_Pos);

        yield return new WaitForSeconds(1.2f);

        MakeBulletAndShotLinear(P1S2_Bullet, LeftFinger_Pos, currentTarget.transform, 300.0f);

        yield return new WaitForSeconds(0.8f);

        currentTarget = oldTarget;
        P1S2_Cooltime_Check = P1S2_Cooltime;
        P1S2 = false;
        EndSkill();
    }

    void UseSkillP1S3RPC()
    {
        StartCoroutine(UseSkillP1S3());
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP1S3", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP1S3()
    {
        PV.RPC("NotifyP1S3", RpcTarget.All);
    }

    [PunRPC]
    void NotifyP1S3()
    {
        StartCoroutine(UseSkillP1S3());
    }
    IEnumerator UseSkillP1S3()
    {
        isCoroutineFinished = false;

        float castDelay = 0.3f;

        anim.SetTrigger("P1S3");

        yield return new WaitForSeconds(castDelay);
        MakeEffectOnBoss(P1S3_Effect, LeftHand_Pos);
        MakeEffectOnBoss(P1S3_Effect, RightHand_Pos);

        yield return new WaitForSeconds(2.5f);
        P1S3_Cooltime_Check = P1S3_Cooltime;
        P1S3 = false;
        EndSkill();
    }

    void UseSkillP2S1RPC()
    {
        StartCoroutine(UseSkillP2S1());
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP2S1", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP2S1()
    {
        PV.RPC("NotifyP2S1", RpcTarget.All);
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

        float animDelay = 1.4f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle2")) animDelay = 1.2f;

        anim.SetTrigger("P2S1");

        yield return new WaitForSeconds(animDelay);
        swordVfx.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        P2S1_Cooltime_Check = P2S1_Cooltime;
        P2S1 = false;
        EndSkill();
    }

    void UseSkillP2S2RPC()
    {
        StartCoroutine(UseSkillP2S2());
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP2S2", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP2S2()
    {
        PV.RPC("NotifyP2S2", RpcTarget.All);
    }

    [PunRPC]
    void NotifyP2S2()
    {
        StartCoroutine(UseSkillP2S2());
    }

    IEnumerator UseSkillP2S2()
    {
        Transform oldTarget = currentTarget;
        currentTarget = FindFarTarget();

        sword.SetActive(false);
        isCoroutineFinished = false;

        string animName = "P2S2";
        anim.SetTrigger(animName);

        float animDelay = 1.4f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle2")) animDelay = 1.2f;
        float castDelay = 4.3f;

        yield return new WaitForSeconds(animDelay);
        MakeEffectOnBoss(P2S2_CastingEffect, RightHand_Pos);

        yield return new WaitForSeconds(castDelay);
        MakeBulletAndShotNav(P2S2_Bullet, RightHand_Pos, currentTarget.transform);

        yield return new WaitForSeconds(2.0f);
        sword.SetActive(true);

        currentTarget = oldTarget;
        P2S2_Cooltime_Check = P2S2_Cooltime;
        P2S2 = false;
        EndSkill();
    }

    void UseSkillP2S3RPC()
    {
        StartCoroutine(UseSkillP2S3());
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP2S3", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP2S3()
    {
        PV.RPC("NotifyP2S3", RpcTarget.All);
    }

    [PunRPC]
    void NotifyP2S3()
    {
        StartCoroutine(UseSkillP2S3());
    }

    IEnumerator UseSkillP2S3()
    {
        isCoroutineFinished = false;

        float castDelay = 1.2f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle2")) castDelay = 1.0f;

        anim.SetTrigger("P2S3");

        yield return new WaitForSeconds(castDelay);
        MakeEffectOnTarget(P2S3_TargettingEffect, this.transform, 54f);

        yield return new WaitForSeconds(0.5f);
        P2S3_Cooltime_Check = P2S3_Cooltime;
        P2S3 = false;
        EndSkill();
    }

    void UseSkillP3S1RPC()
    {
        StartCoroutine(UseSkillP3S1());
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP3S1", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP3S1()
    {
        PV.RPC("NotifyP3S1", RpcTarget.All);
    }

    [PunRPC]
    void NotifyP3S1()
    {
        StartCoroutine(UseSkillP3S1());
    }

    IEnumerator UseSkillP3S1()
    {
        isCoroutineFinished = false;

        float castDelay = 0.3f;

        anim.SetTrigger("P3S1");

        yield return new WaitForSeconds(castDelay);
        MakeEffectOnBoss(P3S1_Effect, LeftHand_Pos);
        MakeEffectOnBoss(P3S1_Effect, RightHand_Pos);

        yield return new WaitForSeconds(2.5f);
        P3S1_Cooltime_Check = P3S1_Cooltime;
        P3S1 = false;
        EndSkill();
    }

    void UseSkillP3S2RPC()
    {
        StartCoroutine(UseSkillP3S2());
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP3S2", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP3S2()
    {
        PV.RPC("NotifyP3S2", RpcTarget.All);
    }

    [PunRPC]
    void NotifyP3S2()
    {
        StartCoroutine(UseSkillP3S2());
    }

    IEnumerator UseSkillP3S2()
    {
        Transform oldTarget = currentTarget;
        currentTarget = FindFarTarget();

        isCoroutineFinished = false;

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

        currentTarget = oldTarget;
        P3S2_Cooltime_Check = P3S2_Cooltime;
        P3S2 = false;
        EndSkill();
    }

    void UseSkillP3S3RPC()
    {
        StartCoroutine(UseSkillP3S3());
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP3S3", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP3S3()
    {
        PV.RPC("NotifyP3S3", RpcTarget.All);
    }

    [PunRPC]
    void NotifyP3S3()
    {
        StartCoroutine(UseSkillP3S3());
    }

    IEnumerator UseSkillP3S3()
    {
        isCoroutineFinished = false;

        float castDelay = 1.2f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle3")) castDelay = 1.0f;
        //else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Chase")) castDelay = 1.0f;

        anim.SetTrigger("P3S3");

        yield return new WaitForSeconds(castDelay);
        MakeEffectOnTarget(P3S3_TargettingEffect, this.transform, 54f);

        yield return new WaitForSeconds(0.5f);
        P3S3_Cooltime_Check = P3S3_Cooltime;
        P3S3 = false;
        EndSkill();
    }

    void UseSkillP3S4RPC()
    {
        StartCoroutine(UseSkillP3S4());
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP3S4", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP3S4()
    {
        PV.RPC("NotifyP3S4", RpcTarget.All);
    }

    [PunRPC]
    void NotifyP3S4()
    {
        StartCoroutine(UseSkillP3S4());
    }

    IEnumerator UseSkillP3S4()
    {
        swordVfx.gameObject.SetActive(true);
        isCoroutineFinished = false;

        float animDelay = 1.4f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle3")) animDelay = 1.2f;

        anim.SetTrigger("P3S4");
        yield return new WaitForSeconds(animDelay);
        swordVfx.gameObject.SetActive(false);

        P3S4_Cooltime_Check = P3S4_Cooltime;
        P3S4 = false;
        EndSkill();
    }

    void UseSkillP3S5RPC()
    {
        StartCoroutine(UseSkillP3S5());
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            PV.RPC("NotifySkillUsedP3S5", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void NotifySkillUsedP3S5()
    {
        PV.RPC("NotifyP3S5", RpcTarget.All);
    }

    [PunRPC]
    void NotifyP3S5()
    {
        StartCoroutine(UseSkillP3S5());
    }

    IEnumerator UseSkillP3S5()
    {
        isCoroutineFinished = false;
        agent.isStopped = true;

        anim.SetTrigger("P3S5");

        float castDelay = 1.8f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) castDelay = 1.6f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Chase")) castDelay = 1.4f;

        yield return new WaitForSeconds(castDelay);
        MakeEffectOnTarget(P1S1_TargettingEffect, this.transform, 52.5f);

        yield return new WaitForSeconds(1.0f);
        P3S5_Cooltime_Check = P3S5_Cooltime;
        P3S5 = false;
        EndSkill();
    }
}

