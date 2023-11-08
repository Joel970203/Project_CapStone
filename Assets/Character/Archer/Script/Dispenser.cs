using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
    public GameObject arrowPrefab;

    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Vector3 fireDirection = transform.forward;
            Arrow arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity).GetComponent<Arrow>();
            arrow.Shot(fireDirection);
        }
    }
}