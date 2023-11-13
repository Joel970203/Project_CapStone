using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Random = UnityEngine.Random;

public class Boss_Skill2 : MonoBehaviour
{
    [SerializeField] private float currentPhase;

    [SerializeField] private Transform P1S2_CastingPosition;

    [SerializeField] private float P1S1_Cooltime; //Phase(P) 1의 Skill(S) 1이란 뜻
    [SerializeField] private float P1S2_Cooltime; //Phase 1의 Skill 2이란 뜻
    [SerializeField] private float P2S1_Cooltime;
    [SerializeField] private float P2S2_Cooltime;
    [SerializeField] private float P3S1_Cooltime;
    [SerializeField] private float P3S2_Cooltime;
    [SerializeField] private float P4S1_Cooltime;
    [SerializeField] private float P4S2_Cooltime;

    private float P1S1_StartTime = -1f;
    private float P1S2_StartTime = -1f;
    private float P2S1_StartTime = -1f;
    private float P2S2_StartTime = -1f;
    private float P3S1_StartTime = -1f;
    private float P3S2_StartTime = -1f;
    private float P4S1_StartTime = -1f;
    private float P4S2_StartTime = -1f;

    [SerializeField] private GameObject P1S1_CastingEffect;
    [SerializeField] private GameObject P1S2_CastingEffect;
    [SerializeField] private GameObject P2S1_CastingEffect;
    [SerializeField] private GameObject P2S2_CastingEffect;
    [SerializeField] private GameObject P3S1_CastingEffect;
    [SerializeField] private GameObject P3S2_CastingEffect;
    [SerializeField] private GameObject P4S1_CastingEffect;
    [SerializeField] private GameObject P4S2_CastingEffect;

    [SerializeField] private GameObject P1S2_Bullet;

    [SerializeField] private GameObject P1S1_TargettingEffect;
    [SerializeField] private GameObject P1S2_TargettingEffect;
    [SerializeField] private GameObject P2S1_TargettingEffect;
    [SerializeField] private GameObject P2S2_TargettingEffect;
    [SerializeField] private GameObject P3S1_TargettingEffect;
    [SerializeField] private GameObject P3S2_TargettingEffect;
    [SerializeField] private GameObject P4S1_TargettingEffect;
    [SerializeField] private GameObject P4S2_TargettingEffect;

    GameObject skillObjects;

    [SerializeField] Transform swordVfx;
    ParticleSystem swordParticle;

    Color color = Color.white;

    private Animator anim;
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;
    const float tau = Mathf.PI * 2;

    private List<GameObject> targets;
    Transform nearTarget;
    Transform currentTarget;

    [SerializeField] float chaseRange; //해당 범위보다 가까이 오면 추격을 시작한다.
    [SerializeField] float chaseDuration; //추격 지속 시간, 해당 초만큼 추격을 함
    float chaseStartTime; //추격을 시작한 시간을 기록
    [SerializeField] float baseAttackProb; //공격시에 기본공격을 할 확률 (스킬쓸 확률 = 1 - BaseAttackProb)
    [SerializeField] float meleeSkillProb; //근접 스킬을 쓸 확률
    [SerializeField] float useSkillInChaseProb; //추격 상황에서 스킬을 쓸 확률

    float distanceToTarget = Mathf.Infinity;

    bool isFinished = true; // 데드락 방지
    bool isCoroutineFinished = true;
    bool isChaseTimerSet = false; //변화여부
    bool isAttacking = false;

    private enum State
    {
        Idle,
        Chase,
        Attack
    }

    State currentState = State.Idle;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        lineRenderer = GetComponent<LineRenderer>();
        targets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        swordVfx.gameObject.SetActive(false);
        swordParticle = swordVfx.GetComponent<ParticleSystem>();
        skillObjects = new GameObject("SkillObjects");
        skillObjects.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        DrawRange(transform.position.x, transform.position.z, chaseRange, 50);

        if (currentTarget != null)
        {
            transform.LookAt(currentTarget); //if (!isAttacking) 
            if (currentState != State.Attack) agent.SetDestination(currentTarget.position); //어택이면 stoppingDistance 에 더 갈 필요 없음
        }

        if (!isFinished || !isCoroutineFinished) return;

        Debug.Log("함수 진입");

        isFinished = false;

        //Debug.Log("State: " + currentState);

        currentPhase = BossObject.instance.GetCurrentPhase();
        nearTarget = FindnearTarget();

        switch (currentState)
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

    void DrawRange(float x, float z, float radius, int vertexs)
    {
        ChangeLineColor();
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

    void ChangeLineColor()
    {
    }

    void JudgeStateInIdle()
    {
        //공격받을 때는 추가적으로 따로 작성

        if (nearTarget != null)
        {
            if (distanceToTarget >= agent.stoppingDistance) SetChase(nearTarget); //공격 범위 밖이면 추격 상태로
            else SetAttack(nearTarget); //안이면 공격 상태로
        }
        //else Debug.Log("범위 내에 플레이어가 없음");
    }

    void JudgeStateInChase()
    {
        Transform nearTarget = FindnearTarget();

        /*Debug.Log("추격시작: " + chaseStartTime);
        float tmp = Time.time - chaseStartTime;
        Debug.Log("추격지속: " + tmp);*/

        if (nearTarget == null) //근처에 타겟도 없고 추격 시간도 끝나면 상태를 Idle로
        {
            if (!isChaseTimerSet)
            {
                chaseStartTime = Time.time; //타이머가 꺼져있다면 추격타이머 설정
                isChaseTimerSet = true;
                return;
            }
            else
            {
                if (Time.time - chaseStartTime >= chaseDuration)
                {
                    isChaseTimerSet = false;
                    SetIdle();
                    return;
                }
                else return;
            }
        }
        else
        {
            isChaseTimerSet = false; //범위 내에 플레이어가 존재하면 타이머는 꺼짐
        }

        if (nearTarget == currentTarget) //가까운 타겟이 현재 타겟과 일치
        {
            /*if (Random.value < useSkillInChaseProb) //일정확률로 원거리 스킬을 사용하여 타겟을 공격하도록
            {
                Debug.Log("추격중 원거리 공격 " + nearTarget.name);
                rangedAttack(); 
                return;
            }*/

            if (distanceToTarget < agent.stoppingDistance) SetAttack(nearTarget); //만일 공격 범위 내에 있으면 타겟을 공격하도록 
            else return;
        }
        else //현재 타겟과 다른 경우 
        {
            if (distanceToTarget < agent.stoppingDistance) SetAttack(nearTarget); //현재 타겟과 달라도 범위내에 있으면 공격
            else currentTarget = nearTarget; //타겟을 바꿔줌
        }
    }

    void JudgeStateInAttack()
    {

        Transform nearTarget = FindnearTarget();

        if (nearTarget != null)
        {
            //공격 범위 내에 플레이어가 있으면 if문 나가도록, 없다면 현재타겟과 일치하는 지 확인 후 제어 
            if (distanceToTarget >= agent.stoppingDistance)
            {
                if (currentTarget == nearTarget) SetChase(nearTarget); //타겟이 변하지 않았으니 추격 상태로
                else SetChase(nearTarget);

                return;
            }
        }
        else
        {   //idle->attack 이던 chase->attack 이던 currentTarget이 존재한채로 오게 됨 nearTarget이 없으면 따라가게 끝 타이머를 키고 chase로 보냄
            chaseStartTime = Time.time;
            isChaseTimerSet = true;
            SetChase(currentTarget);

            return;
        }

        //BaseAttack();
        //StartCoroutine(UseSkillP1S2());
        StartCoroutine(UseSkillP1S1());
        //SkillAttack();

        //if (Random.value < baseAttackProb) BaseAttack();
        //else SkillAttack();
    }

    Transform FindnearTarget()
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
            color = Color.red;
            distanceToTarget = minDistance;
            //Debug.Log("nearTarget: " + nearTarget.name);
        }
        else color = Color.white;

        return nearTarget;
    }

    void FindFarTarget()
    {
        float distance;
        float maxDistance = float.Epsilon;

        foreach (GameObject target in targets)
        {
            distance = Vector3.Distance(this.transform.position, target.transform.position);

            if (distance > maxDistance) maxDistance = distance;
        }
    }

    void SetIdle()
    {
        agent.isStopped = true; //agent를 멈추고 경로 재설정
        agent.ResetPath();

        currentTarget = null;

        anim.SetTrigger("Idle");
        currentState = State.Idle;
    }

    void SetChase(Transform nearTarget)
    {
        currentState = State.Chase; //상태를 추격(Chase) 상태로 바꿈
        anim.SetTrigger("Chase");
        currentTarget = nearTarget;
    }

    void SetAttack(Transform nearTarget)
    {
        chaseStartTime = -1f;
        currentTarget = nearTarget;
        currentState = State.Attack; //공격하도록
    }

    void BaseAttack()
    {
        isAttacking = true;
        swordVfx.gameObject.SetActive(true);
        isCoroutineFinished = false;
        string animName = "BaseAttack";
        RotateBoss(60f);
        anim.SetTrigger(animName);
        StopCoroutine("EndBaseAttack");
        StartCoroutine(EndBaseAttack(2.1f));
    }

    void SkillAttack()
    {
        bool isCooltime = false; //스킬이 사용가능한가 대한 여부 쿨타임이 돌고 있으면 스킬을 쓰면 안됨
        float randProb = Random.value;

        if (randProb < meleeSkillProb) //근접공격 확률에 따라 사용
        {
            isCooltime = MeleeAttack();
        }
        else
        {
            isCooltime = rangedAttack();
        }

        if (isCooltime) BaseAttack(); //쿨타임이 안끝났으면 그냥 기본공격을 한다.

    }

    IEnumerator EndBaseAttack(float length)
    {
        yield return new WaitForSeconds(length);
        swordVfx.gameObject.SetActive(false);
        anim.SetTrigger("Idle");
        isAttacking = false;
        isCoroutineFinished = true;
        Debug.Log("공격애니메이션 종료");
    }

    bool MeleeAttack()
    {
        bool isCooltime = false;
        switch (currentPhase)
        {
            case 1.0f:
                float tmp = Time.time - P1S1_StartTime;
                //Debug.Log("스킬 사용시간" + tmp);
                if (Time.time - P1S1_StartTime >= P1S1_Cooltime || P1S1_StartTime == -1f) UseSkillP1S1(); //쿨타임이 끝났으면 스킬 사용
                else isCooltime = true;
                break;
            case 2.0f:
                if (Time.time - P2S1_StartTime >= P2S1_Cooltime || P1S1_StartTime == -1f) UseSkillP2S1();
                else isCooltime = true;
                break;
            case 3.0f:
                if (Time.time - P3S1_StartTime >= P3S1_Cooltime || P1S1_StartTime == -1f) useSkillP3S1();
                else isCooltime = true;
                break;
            case 4.0f:
                if (Time.time - P4S1_StartTime >= P4S1_Cooltime || P1S1_StartTime == -1f) useSkillP4S1();
                else isCooltime = true;
                break;
        }

        return isCooltime;
    }

    bool rangedAttack()
    {
        bool isCooltime = false;
        switch (currentPhase)
        {
            case 1.0f:
                if (Time.time - P1S2_StartTime >= P1S2_Cooltime) UseSkillP1S2();
                else isCooltime = true;
                break;
            case 2.0f:
                if (Time.time - P2S2_StartTime >= P2S2_Cooltime) useSkillP2S2();
                else isCooltime = true;
                break;
            case 3.0f:
                if (Time.time - P3S2_StartTime >= P3S2_Cooltime) useSkillP3S2();
                else isCooltime = true;
                break;
            case 4.0f:
                if (Time.time - P4S2_StartTime >= P4S2_Cooltime) useSkillP4S2();
                else isCooltime = true;
                break;
        }

        return isCooltime;
    }

    void RotateBoss(float RotateDegree)
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y += RotateDegree;
        transform.rotation = Quaternion.Euler(currentRotation);
    }

    /*
    IEnumerator MakeEffect(Transform castingPosition, GameObject castingEffect, GameObject bullet, float bulletPower,
    Transform targetPosition, GameObject targetEffect, float castDelay, float bulletDelay, float targetPointY, float targetDelay)
    {
        if (castingEffect != null)
        {
            MakeCastEffect(castingEffect, castingPosition);
        }

        yield return new WaitForSeconds(castDelay);

        if (bullet != null)
        {
            MakeAndShotBullet(bullet, castingPosition, targetPosition, bulletPower);
            yield return new WaitForSeconds(bulletDelay);
        }

        if (targetEffect != null)
        {
            Vector3 targetPoint = targetPosition.position;
            targetPoint.y = targetPointY;

            GameObject targetPointEffect = Instantiate(targetEffect, targetPoint, targetEffect.transform.rotation);
            targetPointEffect.transform.SetParent(skillObjects.transform);
        }

        yield return new WaitForSeconds(targetDelay);
        anim.SetTrigger("Idle");
        isAttacking = false;
        isCoroutineFinished = true;
    }*/

    void MakeCastEffect(GameObject castingEffect, Transform castingPosition)
    {
        GameObject CastingEffect = Instantiate(castingEffect, castingPosition.position, castingPosition.transform.rotation);
        CastingEffect.transform.SetParent(castingPosition);
    }

    void MakeBulletAndShotLinear(GameObject bullet, Transform bulletPosition, Transform targetPosition, float bulletPower) //직선으로 발사
    {
        //Make Bullet
        GameObject bulletInstant = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation); //보스의 몸에서 발사
        Rigidbody bulletRigid = bulletInstant.GetComponent<Rigidbody>();

        //Shot Bullet
        Vector3 dirVector = (targetPosition.position - bulletPosition.position).normalized;

        bulletInstant.transform.rotation = Quaternion.LookRotation(dirVector);
        bulletRigid.AddForce(bulletInstant.transform.forward * bulletPower, ForceMode.Impulse);
        //bulletRigid.AddForce(dirVector * bulletPower, ForceMode.Impulse); (이전코드)
    }

    void MakeBulletAndShotProjectile() //포물선으로 발사
    {

    }

    void MakeTargetEffect(GameObject targetEffect, Transform targetPosition, float targetPointY)
    {
        Vector3 targetPoint = targetPosition.position;
        targetPoint.y = targetPointY;

        GameObject targetPointEffect = Instantiate(targetEffect, targetPoint, targetEffect.transform.rotation);
        targetPointEffect.transform.SetParent(skillObjects.transform);
    }

    void EndSkill()
    {
        anim.SetTrigger("Idle");
        isAttacking = false;
        isCoroutineFinished = true;
    }

    /*
    Rigidbody MakeBullet(GameObject bullet, Transform bulletPosition)
    {
        GameObject bulletInstant = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation); //보스의 몸에서 발사
        Rigidbody bulletRigid = bulletInstant.GetComponent<Rigidbody>();

        return bulletRigid;
    }

    void ShotBullet(Transform targetPosition, Transform bulletPosition, Rigidbody bulletRigid, float bulletPower)
    {
        Vector3 dirVector = (targetPosition.position - bulletPosition.position).normalized;
        bulletRigid.AddForce(dirVector * bulletPower, ForceMode.Impulse);
    }
    */

    IEnumerator UseSkillP1S1()
    {
        isCoroutineFinished = false;
        string animName = "P1S1";
        P1S1_StartTime = Time.time;

        float castDelay = 0f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) castDelay = 1.5f;
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Chase")) castDelay = 1.5f;

        anim.SetTrigger(animName);

        yield return new WaitForSeconds(castDelay);
        MakeTargetEffect(P1S1_TargettingEffect, this.transform, 52.5f);

        yield return new WaitForSeconds(2.0f);
        EndSkill();
        //StartCoroutine(MakeEffect(null, null, null, 0f, transform, P1S1_TargettingEffect, 1.7f, 0f, 52.5f, 2.0f));
    }

    IEnumerator UseSkillP1S2()
    {
        isCoroutineFinished = false;
        string animName = "P1S2";
        isAttacking = true;
        //RotateBoss(50f);
        anim.SetTrigger(animName);
        P1S1_StartTime = Time.time;


        float animDelay = 0f;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {

        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Chase"))
        {
            animDelay = 1.5f;
        }

        yield return new WaitForSeconds(animDelay);
        MakeCastEffect(P1S2_CastingEffect, P1S2_CastingPosition);

        
        MakeBulletAndShotLinear(P1S2_Bullet, P1S2_CastingPosition, currentTarget.transform, 300.0f);


        yield return new WaitForSeconds(2.0f);
        EndSkill();
        //StartCoroutine(MakeEffect(P1S2_CastingPosition, P1S2_CastingEffect, P1S2_Bullet, 300.0f, currentTarget.transform, null, 1.5f, 1.0f, 0f, 1.0f));
    }

    void UseSkillP2S1()
    {

    }

    void useSkillP3S1()
    {

    }

    void useSkillP4S1()
    {

    }

    void useSkillP2S2()
    {

    }

    void useSkillP3S2()
    {

    }

    void useSkillP4S2()
    {

    }
}