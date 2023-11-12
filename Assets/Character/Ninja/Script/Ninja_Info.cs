using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja_Info : MonoBehaviour
{
    [SerializeField]
    public int N_HP;

    [SerializeField]
    private int N_AD;

    [SerializeField]
    private int N_armor;

    [SerializeField]
    private GameObject hitEffectPrefab;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //보스에게 근접 공격 피격시 작동하는 이펙트
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Boss"))
        {
            Vector3 characterCenter = transform.position; // 캐릭터 중심점
            Vector3 BossCenter = other.gameObject.transform.position; // 적의 중심점

            // 타격 위치를 평균 위치로 설정
            Vector3 hitPoint = (characterCenter + BossCenter) / 2;

            // 타격 이펙트를 생성하거나 처리
            GameObject spawnedHit =Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);

            Ninja_TakeDamage(20);
        }
    }


    public virtual void Ninja_TakeDamage(int damage)
    {
        if (damage - N_armor > 0)
        {
            N_HP -= damage - N_armor;
            if (N_HP <= 0)
            {
                Ninja_Death();
            }
        }
    }

    private void Ninja_Death()
    {

    }
}
