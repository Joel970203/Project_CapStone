using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public float normalDamage = 20.0f; // 기본 공격 데미지 

    public void PerformAttack()
    {
        float attackDamage = CalculateDamage();

        // Character_Info 스크립트
        Character_Info characterInfo = FindObjectOfType<Character_Info>();

        if (characterInfo != null)
        {
            // Character_Info 스크립트의 HP 변수를 사용
            characterInfo.HP -= (int)attackDamage;
        }
    }

    float CalculateDamage()
    {
        return normalDamage * GetAttackMultiplier();
    }

    float GetAttackMultiplier()
    {
        return 1;
    }
}
