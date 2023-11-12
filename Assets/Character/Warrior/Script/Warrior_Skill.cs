using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior_Skill : Character_Skill
{
    private GameObject weapon;

    [SerializeField] private ParticleSystem weaponParticles;
    [SerializeField] private ParticleSystem Q_Particles;
    [SerializeField] private ParticleSystem W_Particles;
    [SerializeField] private ParticleSystem E_Particles;
    [SerializeField] private ParticleSystem R_Particles;
    public override void Active_Base_Attack()
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
            anim.SetTrigger("Base Attack");
        }
    }
    public override void Active_Q_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            anim.SetTrigger("Q");

            // 1초 대기 후에 파티클 생성
            StartCoroutine(SpawnQParticlesAfterDelay(2.65f));
        }
    }

    private IEnumerator SpawnQParticlesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 spawnPosition = transform.position + transform.forward * 50f;
        GameObject qParticlesObject = Instantiate(Q_Particles, spawnPosition, Quaternion.identity).gameObject;
        Q_Particles.Play();

        // 파티클의 지속 시간 이후에 파티클을 제거
        float particleDuration = Q_Particles.main.duration;
        Destroy(qParticlesObject, particleDuration);
    }

    public override void Active_W_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            anim.SetTrigger("W");

            // 1초 대기 후에 파티클 생성
            StartCoroutine(SpawnWParticlesAfterDelay(1.1f));
        }
    }

    private IEnumerator SpawnWParticlesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 spawnPosition = transform.position + transform.forward * 40f;
        spawnPosition.y += 20f;
        GameObject wParticlesObject = Instantiate(W_Particles.gameObject, spawnPosition, Quaternion.Euler(180f, 0f, 0f));
        W_Particles.Play();

        // 파티클의 지속 시간 이후에 파티클을 제거
        float particleDuration = W_Particles.main.duration;
        Destroy(wParticlesObject, particleDuration);
    }

    public override void Active_E_Skill()
    {
        // 스킬 E 
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            anim.SetTrigger("E");
            E_Particles.Play();

            // 3초 뒤에 파티클을 정지하고 초기화
            Invoke("StopAndClearParticles", 3f);
        }
    }

    private void StopAndClearParticles()
    {
        // 파티클 정지
        E_Particles.Stop();
        E_Particles.Clear();
    }

    public override void Active_R_Skill()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            anim.SetBool("Walk", false);
            agent.ResetPath();
            anim.SetTrigger("R");
            R_Particles.Play();
            Invoke("StopParticles", 10f);
        }
    }

     private void StopParticles()
    {
        // 파티클 정지
        R_Particles.Stop();
        R_Particles.Clear();
    }
}
