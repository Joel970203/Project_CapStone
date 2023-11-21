using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutDamageHandler : MonoBehaviour
{
    [SerializeField] private float outerRadius;
    [SerializeField] private float innerRadius;
    [SerializeField] private float dotDamage;

    private readonly Collider[] outerHits = new Collider[3];
    private readonly Collider[] innerHits = new Collider[3];

    private void FixedUpdate()
    {
        int outerHitsNum = Physics.OverlapSphereNonAlloc(this.transform.position, outerRadius, outerHits, LayerMask.GetMask("Player"));
        //RaycastHit[] outerHits = Physics.SphereCastAll(this.transform.position, outerRadius, Vector3.up, 0f, LayerMask.GetMask("Player"));

        if (outerHitsNum == 0) return;

        //Debug.Log("outer" + outerHits.Length);

        int innerHitsNum = Physics.OverlapSphereNonAlloc(this.transform.position, innerRadius, innerHits, LayerMask.GetMask("Player"));

        //RaycastHit[] innerHits = Physics.SphereCastAll(this.transform.position, innerRadius, Vector3.up, 0f, LayerMask.GetMask("Player"));

        //Debug.Log("inner" + innerHits.Length);

        //foreach (Collider outerHit in outerHits)
        for (int outerNum = 0; outerNum < outerHitsNum; outerNum++)
        {
            bool isInner = false;
            //foreach (Collider innerHit in innerHits)
            for(int innerNum = 0; innerNum < innerHitsNum; innerNum++)
            {
                if (outerHits[outerNum].transform == innerHits[innerNum].transform)
                {
                    isInner = true;
                    break;
                }
            }

            if (!isInner)
            {
                float characterArmor = outerHits[outerNum].transform.GetComponent<Character_Info>().GetArmor();
                outerHits[outerNum].transform.GetComponent<Character_Info>().TakeDamage(dotDamage * Time.deltaTime + characterArmor);
            }
        }
    }
}
