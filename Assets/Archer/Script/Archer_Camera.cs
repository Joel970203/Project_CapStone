using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Camera : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotationOffset = target.transform.position - this.gameObject.transform.position;
        rotationOffset.y=0;
        this.gameObject.transform.forward+=Vector3.Lerp(this.transform.forward,rotationOffset,Time.deltaTime*20f);
    }
}
