using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Attack : MonoBehaviour
{
    [SerializeField] float damage;
    Collider collider;
    
    void Start()
    {
        collider = GetComponent<Collider>();
    }

    void Update()
    {
        if (!collider.enabled)
        {
            collider.enabled = true;
        }
    }

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
