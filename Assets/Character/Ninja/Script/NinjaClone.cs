using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NinjaClone : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    [SerializeField] private ParticleSystem B_Particles;
    [SerializeField] private ParticleSystem W_Particles;
    bool isAttacking = false;
    bool isRSkillActive = false;
    [SerializeField]
    protected float W_Cooltime;

    [HideInInspector]
    public float W_Cooltime_Check;


    public bool W_Skill, Base_Attack;

    protected Animator anim;
    protected UnityEngine.AI.NavMeshAgent agent;
    void Start()
    {
        PV = GetComponent<PhotonView>();
        W_Skill = true;
        Base_Attack = true;
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public IEnumerator DelayedBaseAttack(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 클론의 Base Attack 실행
        Active_Base_Attack();
    }

    public void Active_Base_Attack()
    {
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")) && agent.remainingDistance < 0.1f)
        {
            agent.ResetPath();
            anim.SetBool("Wak", false);
            isAttacking = true;
            PV.RPC("ExecuteBaseAttack", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ExecuteBaseAttack()
    {
        // RPC로 애니메이션 트리거를 설정합니다.
        anim.SetTrigger("Base Attack");

        // 일반 공격을 수행하는 함수를 호출합니다.
        RegularAttack();
    }

    private void RegularAttack()
    {
        StartCoroutine(CreateBParticlesRoutine());
    }

    private IEnumerator CreateBParticlesRoutine()
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

            yield return new WaitForSeconds(0.4f);
            CreateBParticles(targetPosition, 60f, 10f); // X 좌표에 10만큼 더하여 생성
            yield return new WaitForSeconds(0.4f);
            CreateBParticles(targetPosition, 60f, 10f); // X 좌표에 10만큼 더하여 생성
        }

        yield return new WaitForSeconds(1.0f); // 애니메이션 시간에 맞게 조절

        agent.isStopped = false;
        isAttacking = false;
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
    private void ActivateCloneForW()
    {
        // 클론을 활성화
        GameObject ninjaClone = transform.Find("Ninja Clone").gameObject;
        if (ninjaClone != null)
        {
            ninjaClone.SetActive(true);
            // 2초 뒤에 클론 비활성화
            StartCoroutine(DeactivateCloneAfterDuration(ninjaClone, 2.0f));
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
} 