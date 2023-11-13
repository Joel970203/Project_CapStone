using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range_Attack : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private float radius;
    [SerializeField] private int damage;

    void Start()
    {
        StartCoroutine(AfterDelay());
    }

    IEnumerator AfterDelay()
    {
        yield return new WaitForSeconds(delay);

        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, radius, Vector3.up, 0f, LayerMask.GetMask("Player"));
        Debug.Log("맞음" + hits.Length);
        foreach (RaycastHit hit in hits) 
        {
            hit.transform.GetComponent<Character_Info>().TakeDamage(damage);
        }
    }
}
