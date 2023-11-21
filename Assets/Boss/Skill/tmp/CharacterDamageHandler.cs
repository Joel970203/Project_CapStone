using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDamageHandler : MonoBehaviour
{
    private Character_Info characterInfo;
    float CharacterArmor;

    private void Start()
    {
        characterInfo = GetComponent<Character_Info>();
        CharacterArmor = characterInfo.GetArmor();
    }

    private void OnTriggerEnter(Collider attack)
    {
        if (attack.gameObject.CompareTag("BossAttack"))
        {
            HandleDamage(attack);
        }
    }

    private void OnTriggerStay(Collider attack)
    {
        if (attack.gameObject.CompareTag("BossAttack"))
        {
            HandleDamage(attack);
        }
    }

    private void HandleDamage(Collider attack)
    {
        /*
        float damage = attack.gameObject.GetComponent<BossDamage>().damage;
        bool isDot = attack.gameObject.GetComponent<BossDamage>().isDot;

        if (!isDot)
        {
            characterInfo.TakeDamage(damage);
            attack.enabled = false; //지속딜이 아니면 비활성화
        }
        else
        {
            characterInfo.TakeDamage(damage * Time.deltaTime + CharacterArmor); //Dot데미지는 아머를 무시한다.
        }
        */
    }
}
