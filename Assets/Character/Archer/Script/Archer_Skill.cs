using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Archer_Skill : MonoBehaviourPunCallbacks
{
    PhotonView PV;
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

    [SerializeField] private ParticleSystem R_Aura;
    protected Animator anim;
    protected UnityEngine.AI.NavMeshAgent agent;

    bool isLeftMouseButtonPressed = false;
    bool hasShotArrow = false;
    public GameObject arrowPrefab; // 화살 프리팹을 드래그하여 할당
    public GameObject grenadePrefab;
    public Transform arrowSpawnPoint; // 화살이 발사될 위치를 드래그하여 할당
    public Transform GrenadeSpawnPoint;
    public float grenadeThrowDistance = 15f;

    public ParticleSystem Heyste;
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
        if (PV.IsMine)
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
            HandleMouseInput();
        }

    }

    public void ResetCoolDown()
    {
        Q_Skill = true;
        W_Skill = true;
        E_Skill = true;
        R_Skill = true;
    }
    public void Skill_Cooltime_Cal()
    {
        if (Q_Cooltime_Check > 0)
        {
            Q_Cooltime_Check -= Time.deltaTime;
        }
        else
        {
            Q_Cooltime_Check = 0;
            Q_Skill = true;
        }

        if (W_Cooltime_Check > 0)
        {
            W_Cooltime_Check -= Time.deltaTime;
        }
        else
        {
            W_Cooltime_Check = 0;
            W_Skill = true;
        }

        if (E_Cooltime_Check > 0)
        {
            E_Cooltime_Check -= Time.deltaTime;
        }
        else
        {
            E_Cooltime_Check = 0;
            E_Skill = true;
        }

        if (R_Cooltime_Check > 0)
        {
            R_Cooltime_Check -= Time.deltaTime;
        }
        else
        {
            R_Cooltime_Check = 0;
            R_Skill = true;
        }

    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0) && !isLeftMouseButtonPressed)
        {
            isLeftMouseButtonPressed = true;
            hasShotArrow = false;
            Aim_Attack();
        }

        if (Input.GetMouseButton(0) && !hasShotArrow)
        {
            ContinuousAiming();
        }

        if (Input.GetMouseButtonUp(0) && isLeftMouseButtonPressed && !hasShotArrow)
        {
            isLeftMouseButtonPressed = false;
            Active_Base_Attack();
        }
    }

    void ContinuousAiming()
    {
        // 마우스 커서 위치를 기준으로 방향 계산
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;

            // 캐릭터를 마우스 커서 방향으로 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 90도 추가 회전(방향 맞추기 용도)
            targetRotation *= Quaternion.Euler(0, 90, 0);

            transform.rotation = targetRotation;
        }
    }

    public void Aim_Attack()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            return; // Idle 또는 Walk 애니메이션 중일 때만 공격 가능하도록
        }

        anim.SetBool("Walk", false);
        agent.ResetPath();
        anim.SetTrigger("Aiming");

        // 애니메이션 동기화를 위한 RPC 호출
        PV.RPC("SyncAimAnimation", RpcTarget.All);
    }

    [PunRPC]
    private void SyncAimAnimation()
    {
        // 애니메이션 설정
        anim.SetTrigger("Aiming");
    }

    public void Active_Base_Attack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

            // 마우스 커서 위치를 기준으로 방향 계산
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPosition = hit.point;
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0;

                // 캐릭터를 마우스 커서 방향으로 회전
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // 90도 추가 회전
                targetRotation *= Quaternion.Euler(0, 90, 0);

                transform.rotation = targetRotation;
                // 화살 발사 시 목표 지점 설정
                PV.RPC("MoveArrowToTarget", RpcTarget.All, hit.point);
                hasShotArrow = true;
            }
        }
    }

    [PunRPC]
    private void MoveArrowToTarget(Vector3 targetPosition)
    {
        anim.SetTrigger("Shot");
        anim.SetBool("Aiming", false);
        PV.RPC("SimulateArrowMovement", RpcTarget.All, targetPosition);
    }

    [PunRPC]
    IEnumerator SimulateArrowMovement(Vector3 targetPosition)
    {
        yield return new WaitForSeconds(0.05f);
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Rigidbody arrowRigidbody = arrow.GetComponent<Rigidbody>();

        float duration = 1f; // 화살이 목표 지점에 도달하는 시간

        float time = 0f;
        while (time < duration)
        {
            // 새 위치 계산
            Vector3 newPosition = Vector3.Lerp(arrowSpawnPoint.position, targetPosition, time / duration);

            // 방향 계산
            Vector3 direction = newPosition - arrow.transform.position;

            // y축 방향으로 -90도 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, -90, 90);

            // 회전 설정
            arrow.transform.rotation = targetRotation;

            // 화살을 새 위치로 이동
            arrow.transform.position = newPosition;

            time += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(DestroyArrowAfterDelay(arrow, 1f)); // 2초 후에 화살 제거
    }

    IEnumerator DestroyArrowAfterDelay(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(arrow);
    }


    public void Active_Q_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

            // 마우스 커서 위치를 기준으로 방향 계산
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 throwDirection = (hit.point - GrenadeSpawnPoint.position).normalized;

                // 수류탄 던지기를 포톤 RPC로 동기화
                PV.RPC("ThrowGrenade", RpcTarget.All, GrenadeSpawnPoint.position, throwDirection);
            }
        }

    }

    [PunRPC]
    private void ThrowGrenade(Vector3 spawnPosition, Vector3 direction)
    {
        // 수류탄 던지기
        GameObject grenade = Instantiate(grenadePrefab, spawnPosition, Quaternion.identity);

        // Rigidbody에 중력 설정 (중력 사용)
        Rigidbody grenadeRigidbody = grenade.GetComponent<Rigidbody>();
        grenadeRigidbody.useGravity = true;

        anim.SetTrigger("Q");
        // 수류탄에게 힘을 주어 날아가게 하기
        grenadeRigidbody.AddForce(direction * 200f, ForceMode.Impulse);
    }





    public void Active_W_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
            || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            anim.SetTrigger("W");
            agent.speed *= 1.2f;

            // 파티클을 포톤 RPC로 동기화
            PV.RPC("PlayHeysteParticles", RpcTarget.All);
            Invoke("StopHeyste", 10.0f);
        }
    }

    [PunRPC]
    private void PlayHeysteParticles()
    {
        anim.SetTrigger("W");
        Heyste.Play();
    }

    void StopHeyste()
    {
        // 플레이어의 속도 및 파티클을 멈춤
        Heyste.Stop();
        agent.speed /= 1.2f;

        // 파티클 동기화 해제
        PV.RPC("StopHeysteParticles", RpcTarget.All);
    }

    [PunRPC]
    private void StopHeysteParticles()
    {
        Heyste.Stop();
    }


    public void Active_E_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
            || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {
            anim.SetBool("Walk", false);

            // RPC를 통해 화살 발사 동기화
            PV.RPC("ActivateESkillAnimation", RpcTarget.All);
            StartCoroutine(FireArrows(3));

        }
    }

    [PunRPC]
    private void ActivateESkillAnimation()
    {
        anim.SetTrigger("E");
    }

    private IEnumerator FireArrows(int arrowCount)
    {
        for (int i = 0; i < arrowCount; i++)
        {
            // 마우스 커서 위치를 기준으로 방향 계산
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 화살 발사
                PV.RPC("SimulateArrowMovement", RpcTarget.All, hit.point);
                // 대기 시간 (필요에 따라 조절)
                yield return new WaitForSeconds(0.1f);
            }
        }
        hasShotArrow = true;
    }
    public void Active_R_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();

            // 현재 마우스 위치를 기준으로 캐릭터가 바라볼 방향 계산
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPosition = hit.point;

                // 커서 방향으로 시전
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0; // y축 고정
                Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);

                // 캐릭터 방향 조정
                transform.rotation = targetRotation;

                // R 애니메이션 실행 및 RPC 호출
                PV.RPC("ActivateRSkillAnimation", RpcTarget.All);

                // R_Aura 활성화 및 5초 후 정지
                PV.RPC("ActivateRParticleEffect", RpcTarget.All);
                StartCoroutine(StopRParticleEffectAfterDelay(3f));

                // 화살 발사 및 RPC 호출
                StartCoroutine(FireArrows(5));
            }
        }
    }

    [PunRPC]
    private void ActivateRSkillAnimation()
    {
        anim.SetTrigger("R");
    }

    [PunRPC]
    private void ActivateRParticleEffect()
    {
        R_Aura.Play();
    }

    private IEnumerator StopRParticleEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        R_Aura.Stop();
        R_Aura.Clear();
    }

}