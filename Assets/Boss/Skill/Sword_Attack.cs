using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Attack : MonoBehaviour
{
    [SerializeField] float damage;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("감지");
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("사람");
            other.transform.GetComponent<Character_Info>().TakeDamage(damage);
        }
    }
}
