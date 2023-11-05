using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBoltScript : MonoBehaviour
{
    [SerializeField]
    private GameObject ThunderBolt;

    private void Awake() 
    {
        StartCoroutine(MakeThunderBolt(0.2f));
    }
    IEnumerator MakeThunderBolt(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject CastingEffet = Instantiate(ThunderBolt, this.transform.position, Quaternion.identity);
        CastingEffet.transform.parent = this.transform;
        
        Transform childTransform = CastingEffet.transform.Find("LightningStart");
        Vector3 LightingStart=this.transform.position;
        childTransform.position=LightingStart;

        Transform childTransform2 = CastingEffet.transform.Find("LightningEnd");
        Vector3 LightingEnd=this.transform.position;
        LightingEnd.y+=400f;
        childTransform2.position=LightingEnd;
    }
}
