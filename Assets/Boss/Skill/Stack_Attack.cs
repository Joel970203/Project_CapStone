using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack_Attack : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float damagePerSec;

    private void Update()
    {
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, radius, Vector3.up, 0f, LayerMask.GetMask("Player"));

        foreach (RaycastHit hit in hits)
        {
            float armor = hit.transform.GetComponent<Character_Info>().GetArmor();
            Debug.Log("armor" + armor);
            hit.transform.GetComponent<Character_Info>().TakeDamage(damagePerSec * Time.deltaTime + armor);
        }
    }
}
