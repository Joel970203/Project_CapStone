using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ninja_Skill : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    [SerializeField]
    private GameObject weapon;
    [SerializeField] private ParticleSystem weaponParticles;
    [SerializeField] private ParticleSystem B_Particles;
    [SerializeField] private ParticleSystem Q_Particles;
    [SerializeField] private ParticleSystem W_Particles;
    [SerializeField] private ParticleSystem R_Particles;
    bool isAttacking = false;
    bool isRSkillActive = false;
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
    void Start()
    {
        PV = GetComponent<PhotonView>();
        Q_Skill = true;
        W_Skill = true;
        E_Skill = true;
        R_Skill = true;
        Base_Attack = true;
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        Skill_Cooltime_Cal();

        if (Input.GetMouseButtonDown(0))
        {
            Active_Base_Attack();
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
    //공격 애니메이션 작동 중에 클릭 연타시 해당 위치로 이동하는 문제점 발견(의도는 가만히 서서 애니메이션 작동하길 원함)
    //이동하면서 walk 애니메이션이 재생되는 문제를 해결하기 위해 모든 이동 입력을 막는 bool 값 "AllStop"을 설정해줘서 컨트롤 하고자 함.
    protected void StopMove(float delay)
    {
        StartCoroutine(StopClickMove(delay));
    }

    IEnumerator StopClickMove(float delay)
    {
        this.gameObject.GetComponent<ClickMove>().AllStop=true;
        yield return new WaitForSeconds(delay);
        this.gameObject.GetComponent<ClickMove>().AllStop=false;
        yield break;
    }

    public void Active_Base_Attack()
    {
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")) && agent.remainingDistance < 0.1f)
        {
            agent.ResetPath();
            anim.SetBool("Walk", false);

            if (!isAttacking)
            {
                if (isRSkillActive)
                {
                    PV.RPC("ActivateCloneForBaseAttack", RpcTarget.All);
                }
                else
                {
                    isAttacking = true;
                    PV.RPC("SetActiveBaseAttackRoutine", RpcTarget.All);
                }
            }
        }
    }

    [PunRPC]
    private IEnumerator SetActiveBaseAttackRoutine()
    {
        agent.velocity = Vector3.zero;
        agent.isStopped = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion finalRotation = Quaternion.Euler(0, 55f, 0) * targetRotation;
            anim.transform.rotation = finalRotation;
            anim.SetTrigger("Base Attack");

            // 0.1초 간격으로 파티클 생성
            yield return new WaitForSeconds(0.4f);
            CreateBParticles(targetPosition, 60f, 10f); // X 좌표에 10만큼 더하여 생성
            yield return new WaitForSeconds(0.4f);
            CreateBParticles(targetPosition, 60f, 10f); // X 좌표에 10만큼 더하여 생성
        }

        yield return new WaitForSeconds(1.0f); // 애니메이션 시간에 맞게 조절

        agent.isStopped = false;
        isAttacking = false;
    }

    [PunRPC]
    private void ActivateCloneForBaseAttack()
    {
        GameObject ninjaClone = transform.Find("Ninja Clone").gameObject;

        if (ninjaClone != null)
        {
            ninjaClone.SetActive(true);

            // 클론의 NinjaClone 스크립트를 가져옴
            NinjaClone cloneScript = ninjaClone.GetComponent<NinjaClone>();
            if (cloneScript != null)
            {
                // 클론의 메서드 호출
                StartCoroutine(SetActiveBaseAttackRoutine());
                StartCoroutine(cloneScript.DelayedBaseAttack(0.05f));
            }

            StartCoroutine(DeactivateCloneAfterDuration(ninjaClone, 1.0f));
        }
    }

    private IEnumerator DeactivateCloneAfterDuration(GameObject cloneObject, float duration)
    {
        yield return new WaitForSeconds(duration);

        // 클론 비활성화
        if (cloneObject != null)
        {
            cloneObject.SetActive(false);
        }
    }


    void CreateBParticles(Vector3 targetPosition, float yRotation, float xOffset)
    {
        Vector3 spawnPosition = transform.position + transform.forward * 20f;
        spawnPosition.x += xOffset; // X 좌표에 xOffset을 더함
        spawnPosition.y += 20f;

        Quaternion spawnRotation = Quaternion.Euler(0, yRotation, 0);

        GameObject ParticlesObject = Instantiate(B_Particles.gameObject, spawnPosition, spawnRotation);
        var particleSystem = ParticlesObject.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
            ParticleSystem.MainModule mainModule = particleSystem.main;
            float particleDuration = mainModule.duration;
            Destroy(ParticlesObject, particleDuration);
        }
    }
    public void Active_Q_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {       
            anim.SetBool("Walk", false);
            agent.ResetPath();
            GameObject boss = GameObject.FindWithTag("Boss");
            if (boss != null)
            {
                // 보스와 플레이어 간의 거리와 방향 계산
                Vector3 bossPosition = boss.transform.position;
                Vector3 playerPosition = transform.position;
                Vector3 bossDirection = (bossPosition - playerPosition).normalized;
                float skillDistance = 50.0f; // 이동할 거리 (적절한 값 찾기)

                // 보스가 바라보는 방향의 뒤로 이동 위치 계산
                Vector3 movePosition = bossPosition + bossDirection * skillDistance;

                // 이동 위치로 플레이어 이동
                transform.position = movePosition;

                // 보스를 향하도록 회전
                transform.LookAt(boss.transform);
                
                
                photonView.RPC("PlayQParticles", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void PlayQParticles()
    {
        anim.SetTrigger("Q");
        Q_Particles.Play();
        Invoke("StopQParticles", 1f);
    }

    private void StopQParticles()
    {
        Q_Particles.Stop();
        Q_Particles.Clear();
    }

    public void Active_W_Skill()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                anim.SetBool("Walk", false);
                agent.ResetPath();
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                Quaternion finalRotation = Quaternion.Euler(0, 90f, 0) * targetRotation;

                anim.transform.rotation = finalRotation;

                if (isRSkillActive)
                {
                    photonView.RPC("ActivateCloneForW", RpcTarget.All);
                    photonView.RPC("PlayWParticles", RpcTarget.All);
                    photonView.RPC("SpawnDelay", RpcTarget.All, 0.5f, 0.35f, 3);
                }
                else
                {
                    photonView.RPC("PlayWParticles", RpcTarget.All);
                    photonView.RPC("SpawnDelay", RpcTarget.All, 0.5f, 0.35f, 3);
                }
            }
        }
    }



    [PunRPC]
    private void PlayWParticles()
    {
        anim.SetTrigger("W");

        W_Particles.Play();

        float duration = W_Particles.main.duration;
        Invoke("StopWParticles", duration);
    }

    private void StopWParticles()
    {
        W_Particles.Stop();
        W_Particles.Clear();
    }

    [PunRPC]
    private IEnumerator SpawnDelay(float initialDelay, float interval, int spawnCount)
    {
        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < spawnCount; i++)
        {
            yield return new WaitForSeconds(interval);

            Vector3 spawnPosition = transform.position + transform.forward * 10f;
            spawnPosition.y += 20f;

            Quaternion particleRotation = Quaternion.Euler(0f, 60f, 0f);

            GameObject WParticlesObject = Instantiate(W_Particles.gameObject, spawnPosition, particleRotation);
            var particleSystem = WParticlesObject.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
                ParticleSystem.MainModule mainModule = particleSystem.main;
                float particleDuration = mainModule.duration;
                Destroy(WParticlesObject, particleDuration);
            }
        }
    }

    [PunRPC]
    private void ActivateCloneForW()
    {
        // 클론을 활성화
        GameObject ninjaClone = transform.Find("Ninja Clone (W)").gameObject;
        if (ninjaClone != null)
        {
            // 클론 활성화
            ninjaClone.SetActive(true);

            // 클론이 활성화되면 기본 공격 시작 및 클론 비활성화
            StartCoroutine(StartBaseAttackAndDeactivateClone(ninjaClone));
        }
    }

    // 클론이 활성화된 후 기본 공격을 시작하고 일정 시간이 지난 후에 클론을 비활성화하는 코루틴
    private IEnumerator StartBaseAttackAndDeactivateClone(GameObject cloneObject)
    {
        // 클론의 NinjaClone 스크립트를 가져옴
        NinjaClone cloneScript = cloneObject.GetComponent<NinjaClone>();
        if (cloneScript != null)
        {
            // 클론의 W 스킬 메서드 호출
            yield return new WaitForSeconds(0.05f);
            cloneScript.Active_W_Skill();
        }

        yield return new WaitForSeconds(1.5f);

        // 클론 비활성화
        cloneObject.SetActive(false);
    }

    public void Active_E_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPosition = hit.point;
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion finalRotation = Quaternion.Euler(0, 55f, 0) * targetRotation;
                transform.rotation = finalRotation;
    
                weaponParticles.Play();
                Invoke("StopweaponParticles", 10.0f);

                // 다른 플레이어에게 E 스킬을 동기화하기 위해 Photon RPC 호출
                photonView.RPC("PlayWeaponParticles", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void PlayWeaponParticles()
    {
        anim.SetTrigger("E");
        // 해당 플레이어의 무기 파티클 실행
        weaponParticles.Play();

        // 이펙트 재생 시간동안 기다린 후에 파티클 정리
        float duration = weaponParticles.main.duration;
        Invoke("StopWeaponParticles", duration);
    }
    public void Active_R_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPosition = hit.point;
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);
                transform.rotation = targetRotation;
                photonView.RPC("ActivateRSkill", RpcTarget.All, targetPosition);
            }
        }
    }

    [PunRPC]
    private void ActivateRSkill(Vector3 targetPosition)
    {
        anim.SetTrigger("R");
        isRSkillActive = true;
        R_Particles.Play();
        StartCoroutine(DeactivateRSkillAfterDurationCoroutine()); // R 스킬 지속 시간 후에 비활성화
    }

    private IEnumerator DeactivateRSkillAfterDurationCoroutine()
    {
        yield return new WaitForSeconds(20.0f); // 20초 지연

        isRSkillActive = false;

        // R 스킬 파티클 정지 및 클리어
        if (R_Particles != null)
        {
            R_Particles.Stop();
            R_Particles.Clear();
        }
    }

    

} 
