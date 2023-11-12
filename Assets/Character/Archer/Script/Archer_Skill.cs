using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Skill : MonoBehaviour 
{
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

                anim.SetTrigger("Shot");
                anim.SetBool("Aiming", false);

                // 화살 발사 시 목표 지점 설정
                StartCoroutine(MoveArrowToTarget(hit.point));
                hasShotArrow = true;
            }
        }
    }

    IEnumerator MoveArrowToTarget(Vector3 targetPosition)
    {
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
        StartCoroutine(DestroyArrowAfterDelay(arrow, 2f)); // 2초 후에 화살 제거
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

                // 수류탄 던지기
                GameObject grenade = Instantiate(grenadePrefab, GrenadeSpawnPoint.position, Quaternion.identity);

                // Rigidbody에 중력 설정 (중력 사용)
                Rigidbody grenadeRigidbody = grenade.GetComponent<Rigidbody>();
                grenadeRigidbody.useGravity = true;

                // 수류탄에게 힘을 주어 날아가게 하기
                grenadeRigidbody.AddForce(throwDirection * 200f, ForceMode.Impulse);
            }
        }
        anim.SetTrigger("Q"); 
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
            Heyste.Play();
            Invoke("StopHeyste", 10.0f);
        }
    }

    void StopHeyste()
    {
        Heyste.Stop();
        agent.speed /= 1.2f;
    }


    public void Active_E_Skill()
    {  
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
            || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {
            anim.SetBool("Walk", false);
            //agent.ResetPath();
            anim.SetTrigger("E");

            // 3번 연속 화살 발사
            StartCoroutine(FireArrows(3));
        }    
    }

    private IEnumerator FireArrows(int arrowCount)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < arrowCount; i++)
        {
            // 마우스 커서 위치를 기준으로 방향 계산
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 화살 발사
                StartCoroutine(MoveArrowToTarget(hit.point));

                // 대기 시간 (필요에 따라 조절)
                yield return new WaitForSeconds(0.1f);
            }
        }
        hasShotArrow = true;
    }
    public void Active_R_Skill() 
    {
        agent.ResetPath();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
            || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPosition = hit.point;

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                {
                    anim.SetBool("Walk", false);
                    agent.ResetPath();

                    // 커서 방향으로 시전 
                    Vector3 direction = targetPosition - transform.position;
                    direction.y = 0; // y축고정
                    Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);

                    transform.rotation = targetRotation;
                    anim.SetTrigger("R");
                }
            }
        }
    }

}