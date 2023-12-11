using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealCollider : MonoBehaviour
{
    CapsuleCollider capsuleCollider;

    public bool isAdapted;
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        isAdapted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAdapted)
        {
            capsuleCollider.enabled = true;
        }
    }
}
