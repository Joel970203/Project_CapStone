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

            // Base Attack 애니메이션이 시작될 때 Invoke 함수를 사용하여 0.5초 후에 ParticleSystem을 실행하고, 0.8초 후에 다시 실행
            StartCoroutine(BaseAttackP(0.3f,1.0f,2));
            anim.SetTrigger("Base Attack");
        }
    }

    private IEnumerator BaseAttackP(float initialDelay, float interval, int spawnCount)
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



    public override void Active_Q_Skill()
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
                Quaternion finalRotation = Quaternion.Euler(0,0,0) * targetRotation;
                transform.rotation = finalRotation;
                anim.SetTrigger("Q");
                StartCoroutine(SpawnQParticlesAfterDelay(2.65f));
            }
        }
    }
    private IEnumerator SpawnQParticlesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 spawnPosition = transform.position + transform.forward * 100f;
        GameObject qParticlesObject = Instantiate(Q_Particles, spawnPosition, Quaternion.identity).gameObject;

        var qParticles = qParticlesObject.GetComponent<ParticleSystem>();
        if (qParticles != null)
        {
            qParticles.Play();

            // 캐릭터가 바라보는 방향으로 파티클을 길게 설정하기 위해 파티클의 Z 축 스케일을 더 크게 설정
            Vector3 particleScale = qParticles.transform.localScale;
            particleScale.x *= 2.5f; // 여기서 5는 예시이며, 원하는 길이에 맞게 조정 가능

            qParticles.transform.localScale = particleScale;

            // 파티클의 지속 시간 이후에 파티클을 제거
            ParticleSystem.MainModule mainModule = qParticles.main;
            float particleDuration = mainModule.duration;
            Destroy(qParticlesObject, particleDuration);
        }
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
/*
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
                Quaternion finalRotation = Quaternion.Euler(0,0,0) * targetRotation;
                transform.rotation = finalRotation;
                anim.SetTrigger("Q");
                StartCoroutine(SpawnQParticlesAfterDelay(2.65f));
            }
*/
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
