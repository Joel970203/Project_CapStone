using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1S1_Script : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private int damage;
    private bool isCastDelay = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AfterDelay());
    }

    IEnumerator AfterDelay()
    {
        yield return new WaitForSeconds(delay);
        isCastDelay = false;
    }
    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if(!isCastDelay && other.CompareTag("Player"))
        {
            Character_Info player = other.GetComponent<Character_Info>();

            player.TakeDamage(damage);
        }   
    }
}
