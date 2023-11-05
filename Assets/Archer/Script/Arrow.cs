using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private bool isShot = true;
    private Vector3 direction;
    public float speed = 10f;

    //mathf.lerp

    public void Shot(Vector3 dir)
    {
        direction = dir;
        isShot = true;
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        if(isShot)
        {
            transform.Translate(direction * Time.deltaTime * speed);
        }
    }
}