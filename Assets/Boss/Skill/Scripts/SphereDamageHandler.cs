using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDamageHandler : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private bool isDot;
    [SerializeField] private float radius;

    private readonly Collider[] hits = new Collider[3];
    
    private void FixedUpdate()
    {
        int hitsNum = Physics.OverlapSphereNonAlloc(this.transform.position, radius, hits, LayerMask.GetMask("Player"));

        if (hitsNum == 0) return;

        for (int num = 0; num < hitsNum; num++)
        {
            if (!isDot)
            {
                //Debug.Log(damage);
                hits[num].transform.GetComponent<Character_Info>().TakeDamage(damage);
                if (num == hitsNum - 1) this.enabled = false; //범위 내 플레이어에게 데미지를 다 줬으면 1회성 데미지이기에 스크립트 비활성화
            }
            else
            {
                float characterArmor = hits[num].transform.GetComponent<Character_Info>().GetArmor();
                hits[num].transform.GetComponent<Character_Info>().TakeDamage(damage * Time.deltaTime + characterArmor); //Dot데미지는 아머를 무시한다.
            }
        }
    }
}
