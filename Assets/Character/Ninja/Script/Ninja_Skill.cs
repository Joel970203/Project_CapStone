using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja_Skill : Character_Skill
{
    [SerializeField]
    private GameObject weapon;

    public GameObject shadowClonePrefab; 
    public float cloneDuration = 20f; // 분신이 유지될 시간
    [SerializeField] private ParticleSystem weaponParticles;
    [SerializeField] private ParticleSystem B_Particles;
    [SerializeField] private ParticleSystem Q_Particles;
    [SerializeField] private ParticleSystem W_Particles;
    [SerializeField] private ParticleSystem R_Particles;
    bool isAttacking = false;

    public override void Active_Base_Attack()
    {
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")) && agent.remainingDistance < 0.1f)
        {
            agent.ResetPath();
            anim.SetBool("Walk", false);

            if (!isAttacking)
            {
                isAttacking = true;
                StartCoroutine(SetActiveBaseAttack());
            }
        }
    }

    IEnumerator SetActiveBaseAttack()
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




    public override void Active_Q_Skill()
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
                
                anim.SetTrigger("Q");
                Invoke("PlayQParticles",0.2f); 
            }
        }
    }
    private void PlayQParticles()
    {
        Q_Particles.Play();
        Invoke("StopQParticles", 1f);
        
    }
    private void StopQParticles()
    {
        Q_Particles.Stop();
        Q_Particles.Clear();
    }


    public override void Active_W_Skill()
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

                // 시계 방향으로 55도 회전
                Quaternion finalRotation = Quaternion.Euler(0, 90f, 0) * targetRotation;

                // 공격 모션 방향을 설정하고 공격 애니메이션 실행
                anim.transform.rotation = finalRotation;

                anim.SetTrigger("W");
                W_Particles.Play();
                StartCoroutine(SpawnDelay(0.5f, 0.35f, 3)); 
            }
        }
    }
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

    public override void Active_E_Skill()
    {
        // 스킬 E 실행
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
                anim.SetTrigger("E");
                weaponParticles.Play();
                Invoke("StopweaponParticles", 10.0f);
            }
        }
    }

    
    public override void Active_R_Skill()
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

                anim.SetTrigger("R");
                StartCoroutine(SpawnShadowClone(targetPosition));
                Invoke("PlayWParticles", 1.5f);
            }
        }
    }

    private void PlayWParticles()
    {
        R_Particles.Play();
        Invoke("StopWParticles", 2f);
    }

    private void StopWParticles()
    {
        R_Particles.Stop();
        R_Particles.Clear();
    }

    IEnumerator SpawnShadowClone(Vector3 targetPosition)
    {
        // 1.5초 뒤에 분신 생성
        yield return new WaitForSeconds(1.5f);

        // 분신 생성
        GameObject shadowClone = Instantiate(shadowClonePrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(cloneDuration - 1.5f);

        // 분신 제거
        Destroy(shadowClone);
    }
}