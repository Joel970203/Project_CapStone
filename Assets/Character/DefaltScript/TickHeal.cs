using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickHeal : MonoBehaviour
{
    // Start is called before the first frame update
    public Collider collider;
    public float delay;
    void Start()
    {
        collider= GetComponent<Collider>();
        StartCoroutine(RegenerateCollider(delay));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator RegenerateCollider(float delay)
    {
        
        while (true)
        {
            yield return new WaitForSeconds(delay);
            collider.enabled=true;
        }
    }
}
