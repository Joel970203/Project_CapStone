using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastDelay_Controller : MonoBehaviour
{
    [SerializeField] float castDelay;
    [SerializeField] string scriptName;

    private void Start()
    {
        StartCoroutine(AfterDelay());
    }

    IEnumerator AfterDelay() 
    {
        yield return new WaitForSeconds(castDelay);
        (this.GetComponent(scriptName) as MonoBehaviour).enabled = true;
    }
}
