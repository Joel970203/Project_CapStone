using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDamageHandler : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private bool isDot;

    private void OnParticleCollision(GameObject other)  
    {
        if (other.CompareTag("Player"))
        {
            HandleDamage(other.transform);
        }
    }

    private void HandleDamage(Transform other)
    {
        if (!isDot) //단일 타겟 스킬 대상으로만 사용할것
        {
            other.GetComponent<Character_Info>().TakeDamage(damage);
            this.enabled = false; //지속딜이 아니면 비활성화
        }
        else
        {
            float characterArmor = other.GetComponent<Character_Info>().GetArmor();
            other.GetComponent<Character_Info>().TakeDamage(damage * Time.deltaTime + characterArmor); //Dot데미지는 아머를 무시한다.
        }
    }
}
