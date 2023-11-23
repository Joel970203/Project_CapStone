using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Warrior_Skill : MonoBehaviourPunCallbacks
{

    PhotonView PV;
    private GameObject weapon;

    [SerializeField] private ParticleSystem weaponParticles;
    [SerializeField] private ParticleSystem Q_Particles;
    [SerializeField] private ParticleSystem W_Particles;
    [SerializeField] private ParticleSystem E_Particles;
    [SerializeField] private ParticleSystem R_Particles;

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
                transform.rotation = targetRotation;
            }

            // 애니메이션을 동기화하는 RPC 호출
            PV.RPC("SyncActiveBaseAttack", RpcTarget.All);
        }
    }

    [PunRPC]
    private void SyncActiveBaseAttack()
    {
        // 애니메이션을 재생하는 메서드 호출
        PlayBaseAttackAnimation();
    }

    private void PlayBaseAttackAnimation()
    {
        // Base Attack 애니메이션 재생
        anim.SetTrigger("Base Attack");
        StartCoroutine(SyncBaseAttackParticles(0.3f, 1.0f, 2));
    }

    private IEnumerator SyncBaseAttackParticles(float initialDelay, float interval, int spawnCount)
    {
        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < spawnCount; i++)
        {
            yield return new WaitForSeconds(interval);

            Vector3 spawnPosition = transform.position + transform.forward * 50f;
            spawnPosition.y += 20f;

            Quaternion particleRotation = Quaternion.identity;

            if (i == 0)
            {
                particleRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
            }
            else if (i == 1)
            {
                particleRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
            }

            // 파티클 생성
            GameObject ParticlesObject = Instantiate(weaponParticles.gameObject, spawnPosition, particleRotation);
            var particleSystem = ParticlesObject.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
                ParticleSystem.MainModule mainModule = particleSystem.main;
                float particleDuration = mainModule.duration;
                Destroy(ParticlesObject, particleDuration);
            }
        }
    }



    public void Active_Q_Skill()
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
                Quaternion finalRotation = Quaternion.Euler(0, 0, 0) * targetRotation;
                transform.rotation = finalRotation;
                PV.RPC("SpawnQParticles", RpcTarget.AllViaServer, transform.position, transform.forward);
            }
        }
    }

    [PunRPC]
    private void SpawnQParticles(Vector3 position, Vector3 forward)
    {
        anim.SetTrigger("Q");
        StartCoroutine(SyncQParticles(position, forward, 2.65f));
    }

    private IEnumerator SyncQParticles(Vector3 position, Vector3 forward, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 spawnPosition = position + forward * 100f;
        GameObject qParticlesObject = Instantiate(Q_Particles, spawnPosition, Quaternion.identity).gameObject;

        var qParticles = qParticlesObject.GetComponent<ParticleSystem>();
        if (qParticles != null)
        {
            qParticles.Play();
            Vector3 particleScale = qParticles.transform.localScale;
            particleScale.x *= 2.5f; // 파티클의 길이 조정
            qParticles.transform.localScale = particleScale;

            ParticleSystem.MainModule mainModule = qParticles.main;
            float particleDuration = mainModule.duration;
            Destroy(qParticlesObject, particleDuration);
        }
    }




    public void Active_W_Skill()
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
                Quaternion finalRotation = Quaternion.Euler(0, 0, 0) * targetRotation;
                transform.rotation = finalRotation;
                PV.RPC("SpawnWParticles", RpcTarget.AllViaServer, transform.position, transform.forward);
            }
        }
    }

    [PunRPC]
    private void SpawnWParticles(Vector3 position, Vector3 forward)
    {
        anim.SetTrigger("W");
        StartCoroutine(SyncWParticles(position, forward, 1.1f));
    }

    private IEnumerator SyncWParticles(Vector3 position, Vector3 forward, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 spawnPosition = position + forward * 40f;
        spawnPosition.y += 20f;
        GameObject wParticlesObject = Instantiate(W_Particles.gameObject, spawnPosition, Quaternion.Euler(180f, 0f, 0f));
        var wParticles = wParticlesObject.GetComponent<ParticleSystem>();
        
        if (wParticles != null)
        {
            wParticles.Play();
            float particleDuration = wParticles.main.duration;
            Destroy(wParticlesObject, particleDuration);
        }
    }

    public void Active_E_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            PV.RPC("SyncEParticles", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    private void SyncEParticles()
    {
        anim.SetTrigger("E");
        E_Particles.Play();
        Invoke("StopAndClearEParticles", 3f);
    }

    // 포톤 RPC로 호출되는 메서드 내에서 E 파티클을 정지하고 초기화.
    private void StopAndClearEParticles()
    {
        E_Particles.Stop();
        E_Particles.Clear();
    }

    public void Active_R_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            PV.RPC("SyncRParticles", RpcTarget.AllViaServer);
            Invoke("StopParticles", 10f);
        }
    }

    // 포톤 RPC로 클라이언트들에게 R 파티클을 재생하도록 동기화
    [PunRPC]
    private void SyncRParticles()
    {
        anim.SetTrigger("R");
        R_Particles.Play();
    }

    // 포톤 RPC로 호출되는 메서드 내에서 R 파티클을 정지하고 초기화
    private void StopParticles()
    {
        // 파티클 정지
        R_Particles.Stop();
        R_Particles.Clear();
    }
}
