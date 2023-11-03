using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Skill : MonoBehaviour
{
    Animator anim;
    private CharacterController characterController;
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

    [SerializeField]
    private float baseDamage = 10.0f; // 기본 공격 데미지

    private float damageMultiplier = 1.0f; // 데미지 배율
    private bool isBossBehind = false; // 보스가 뒤에 있는지 여부

    private bool isWActive = false;
    private float wSkillTimer = 0f;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        // 모든 스킬을 사용 가능으로 설정
        Q_Skill = true;
        W_Skill = true;
        E_Skill = true;
        R_Skill = true;
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


        if (isWActive)
        {
            wSkillTimer -= Time.deltaTime;

            if (wSkillTimer <= 0)
            {
                // 10초 후에 원래 상태로 복원
                characterController.Move(Vector3.forward * Time.deltaTime);
                anim.speed = 1.0f;
                isWActive = false;
            }
        }
    }

    public virtual void Active_Q_Skill()
    {
        agent.ResetPath();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
        || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {       
            anim.SetTrigger("Q");
        }
        
    }
    public virtual void Active_W_Skill()
    { 
        agent.ResetPath();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
        || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {
            anim.SetTrigger("W");
            characterController.Move(Vector3.forward * Time.deltaTime * 1.2f);
            anim.speed = 1.2f;
            isWActive = true;
            wSkillTimer = 10.0f;
        }

    }
    public virtual void Active_E_Skill() 
    {  
        agent.ResetPath();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
        || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {
            anim.SetTrigger("E");
        }

    }    
    public virtual void Active_R_Skill() 
    {
        agent.ResetPath();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")
        || anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Aiming Walk"))
        {
            anim.SetTrigger("R");
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

    // OnTriggerEnter로 보스와 충돌했을 때 호출됨
  /*  void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("boss"))
        {
            Vector3 bossDirection = other.transform.position - transform.position;

            // 거리를 확인하여 데미지 배율을 조절
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
*/
    // 데미지 계산 함수
    public float CalculateDamage()
    {
        // 실제 데미지 계산
        float damage = baseDamage * damageMultiplier;
        return damage;
    }
}
