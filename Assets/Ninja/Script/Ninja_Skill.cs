using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja_Skill : MonoBehaviour
{
    Animator anim;
    UnityEngine.AI.NavMeshAgent agent;
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

    [HideInInspector]
    public bool Q_Skill;
    [HideInInspector]
    public bool W_Skill;
    [HideInInspector]
    public bool E_Skill;
    [HideInInspector]
    public bool R_Skill;

    public GameObject weapon;
    private ParticleSystem weaponParticles;

    [SerializeField]
    private float baseDamage = 10.0f; // 기본 공격 데미지

    private float damageMultiplier = 1.0f; // 데미지 배율
    private bool isBossBehind = false; // 보스가 뒤에 있는지 여부
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        // 모든 스킬을 사용 가능으로 설정
        Q_Skill = true;
        W_Skill = true;
        E_Skill = true;
        R_Skill = true;

        weaponParticles = weapon.GetComponent<ParticleSystem>();
        weaponParticles.Stop();
    }

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
    }

    public virtual void Active_Q_Skill()
    {
        agent.ResetPath();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {       
            GameObject boss = GameObject.FindWithTag("Boss");
            if (boss != null)
            {
                // 보스와 플레이어 간의 거리와 방향 계산
                Vector3 bossPosition = boss.transform.position;
                Vector3 playerPosition = transform.position;
                Vector3 bossDirection = (bossPosition - playerPosition).normalized;
                float skillDistance = 50.0f; // 이동할 거리 (적절한 값찾기)

                // 보스가 바라보는 방향의 뒤로 이동 위치 계산
                Vector3 movePosition = bossPosition + bossDirection * skillDistance;

                // 이동 위치로 플레이어 이동
                transform.position = movePosition;

                // 스킬 애니메이션을 실행
                anim.SetTrigger("Q");
            }
        }
        
    }
    
    public virtual void Active_W_Skill() 
    {
      
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetTrigger("W");
            weaponParticles.Play();
        }
        agent.ResetPath();
    }
    public virtual void Active_E_Skill() 
    { 
        
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetTrigger("E");
        }
        agent.ResetPath();
    }
    public virtual void Active_R_Skill() 
    {
         if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
         {
            anim.SetTrigger("R");
         }
         agent.ResetPath();
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

    // OnTriggerEnter로 보스와 충돌했을 때 호출됨
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("boss"))
        {
            Vector3 bossDirection = other.transform.position - transform.position;

            // 방향을 확인하여 데미지 배율을 조절
            if (Vector3.Dot(bossDirection, transform.forward) < 0)
            {
                // 보스가 뒤를 바라보는 경우
                damageMultiplier = 1.3f;
                isBossBehind = true;
            }
            else
            {
                // 보스가 플레이어를 바라보는 경우
                damageMultiplier = 1.0f;
                isBossBehind = false;
            }
        }
    }

    // OnTriggerExit로 보스와 충돌에서 벗어날 때 호출됨
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("boss"))
        {
            isBossBehind = false;
        }
    }

    // 데미지 계산 함수
    public float CalculateDamage()
    {
        // 방향을 확인하여 데미지 배율을 조절
        if (isBossBehind)
        {
            // 보스가 뒤를 바라보는 경우
            damageMultiplier = 1.3f;
        }
        else
        {
            // 보스가 플레이어를 바라보는 경우
            damageMultiplier = 1.0f;
        }

        // 실제 데미지 계산
        float damage = baseDamage * damageMultiplier;
        return damage;
    }
}