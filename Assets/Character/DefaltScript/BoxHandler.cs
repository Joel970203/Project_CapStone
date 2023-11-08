using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxHandler : MonoBehaviour
{
    [SerializeField] public int HP;

    private void OnTriggerEnter(Collider attack)
    {
        if (attack.gameObject.CompareTag("PlayerAttack"))
        {
            SkillDamage skillDamage = attack.gameObject.GetComponent<SkillDamage>();

            if (skillDamage != null)
            {
                float damage = skillDamage.damage;
                Debug.Log("피격: " + damage);
                HP -= (int)damage; // 보스의 HP에서 데미지를 뺍니다.

                if (HP <= 0)
                {
                    // Box가 파괴될 때의 처리
                    Destroy(gameObject);
                }
            }
        }
    }
}
