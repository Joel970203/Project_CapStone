using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDamageHandler : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private bool isDot;

    private void OnTriggerEnter(Collider other)  
    {
        if(isDot) return;
        if (other.gameObject.CompareTag("Player"))
        {
            HandleDamage(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(!isDot) return;
        if (other.gameObject.CompareTag("Player"))
        {
            HandleDamage(other);
        }
    }

    private void HandleDamage(Collider other)
    {
        if (!isDot) //단일 타겟 스킬 대상으로만 사용할것
        {
            other.transform.GetComponent<Character_Info>().TakeDamage(damage);
            this.enabled = false; //지속딜이 아니면 비활성화
        }
        else
        {
            float characterArmor = other.transform.GetComponent<Character_Info>().GetArmor();
            other.transform.GetComponent<Character_Info>().TakeDamage(damage * Time.deltaTime + characterArmor); //Dot데미지는 아머를 무시한다.
        }
    }
}
