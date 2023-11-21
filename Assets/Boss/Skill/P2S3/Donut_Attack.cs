using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Donut_Attack : MonoBehaviour
{
    [SerializeField] private float outerRadius;
    [SerializeField] private float innerRadius;
    [SerializeField] private float dotDamage;

    private void FixedUpdate()
    {
        RaycastHit[] outerHits = Physics.SphereCastAll(this.transform.position, outerRadius, Vector3.up, 0f, LayerMask.GetMask("Player"));

        //if (outerHits.Length == 0) return;

        Debug.Log("outer" + outerHits.Length);

        RaycastHit[] innerHits = Physics.SphereCastAll(this.transform.position, innerRadius, Vector3.up, 0f, LayerMask.GetMask("Player"));

        Debug.Log("inner" + innerHits.Length);

        foreach (RaycastHit outerHit in outerHits)
        {
            bool isInner = false;
            foreach (RaycastHit innerHit in innerHits)
            {
                if (outerHit.transform == innerHit.transform)
                {
                    isInner = true;
                    break;
                }
            }

            if (!isInner)
            {
                float armor = outerHit.transform.GetComponent<Character_Info>().GetArmor();
                outerHit.transform.GetComponent<Character_Info>().TakeDamage(dotDamage * Time.deltaTime + armor);
            }
        }
    }

}
