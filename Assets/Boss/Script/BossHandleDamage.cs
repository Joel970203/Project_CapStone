using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamageHandler : MonoBehaviour
{
    public float headDamageMultiplier = 1.3f; // 머리에 대한 데미지 배수
    private Boss_Info bossInfo; // Boss_Info 스크립트에 대한 참조

    private bool headHit = false; // 머리 트리거를 했는지 여부를 저장

    private void Start()
    {
        bossInfo = GetComponent<Boss_Info>(); // Boss_Info 스크립트
    }

    private void OnTriggerEnter(Collider attack)
    {
        if (attack.gameObject.CompareTag("PlayerAttack"))
        {

            if (IsHeadCollider() && !headHit)
            {
                // 머리에 맞았을 때
                Debug.Log("머리");
                headHit = true; // 머리 트리거를 했다고 표시
                float headDamage = CalculateDamage(attack) * headDamageMultiplier;
                HandleDamage(headDamage);
            }
            else
            {
                // 다른 부분에 맞았을 때
                Debug.Log("몸통");
                float damage = CalculateDamage(attack);
                Debug.Log("피격");
                Debug.Log(damage);
                HandleDamage(damage);
            }
            //attack.gameObject.SetActive(false);
            attack.enabled = false;
        }
    }

    private void OnTriggerStay(Collider attack)
    {
        if (IsHeadCollider() && !headHit)
        {
            // 머리에 맞았을 때
            Debug.Log("머리");
            headHit = true; // 머리 트리거를 했다고 표시
            float headDamage = CalculateDamage(attack) * headDamageMultiplier;
            HandleDamage(headDamage);
        }
        else
        {
            // 다른 부분에 맞았을 때
            Debug.Log("몸통");
            float damage = CalculateDamage(attack);
            HandleDamage(damage);
        }
        //attack.gameObject.SetActive(false);
        attack.enabled = false;
    }

    private bool IsHeadCollider()
    {
        return gameObject.layer == LayerMask.NameToLayer("HeadLayer");
    }

    private float CalculateDamage(Collider attack)
    {
        if(attack.gameObject.GetComponent<SkillDamage>())
        {
            return attack.gameObject.GetComponent<SkillDamage>().damage;
        }
        return attack.CompareTag("PlayerAttack") ? 10f : 0f;
    }

    private void HandleDamage(float damage)
    {
        if (bossInfo != null)
        {
            bossInfo.HP -= (int)damage; // 보스의 체력을 감소
            if (bossInfo.HP <= 0)
            {
                bossInfo.Death(); // 보스의 체력이 0 이하로 떨어졌을 때 처리를 수행.
            }
        }
    }

}

