using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_Attack : MonoBehaviour
{
    [SerializeField] float damagePerSec;

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("감지");
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("사람");
            other.transform.GetComponent<Character_Info>().TakeDamage(damagePerSec * Time.deltaTime);
        }
    }
}
