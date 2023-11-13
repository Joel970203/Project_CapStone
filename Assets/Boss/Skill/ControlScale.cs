using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScale : MonoBehaviour
{
    [SerializeField] float increaseRate;
    [SerializeField] float maxScale;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.localScale.x <= maxScale) 
        {
            transform.localScale += new Vector3(increaseRate, increaseRate, increaseRate) * Time.deltaTime;
        }
    }
}
