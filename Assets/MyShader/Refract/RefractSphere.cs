using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractSphere : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] float rotateSpeed = 10.0f;
    [SerializeField] float refractDuration = 10.0f;
    [SerializeField] float refractWait = 20.0f;

    void Start()
    {
        StartCoroutine(DelayRefract());
    }

    void Update()
    {   
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        if(boss.GetComponentInParent<Boss_Info>().HP <= 100)
        {
            StopAllCoroutines();
            StartCoroutine(FinalExpand());
        }
    }

    IEnumerator DelayRefract()
    {
        yield return new WaitForSeconds(10.0f);
        StartCoroutine(Expand());
    }

    IEnumerator Expand()
    {
        Vector3 shrinkSize = transform.localScale;
        Vector3 expandSize = new Vector3(7.5f, 7.5f, 7.5f);

        float endTime = 0.61f;
        float startTime = 0.1f;

        while (startTime <= endTime)
        {
            float lerpRate = startTime / endTime;
            transform.localScale = Vector3.Lerp(shrinkSize, expandSize, lerpRate);
            startTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(refractDuration);
        StartCoroutine(Shrink());
    }

    IEnumerator FinalExpand()
    {
        Vector3 shrinkSize = transform.localScale;
        Vector3 expandSize = new Vector3(50.0f, 50.0f, 50.0f);

        float endTime = 60.51f;
        float startTime = 0.1f;

        while (startTime <= endTime)
        {
            float lerpRate = startTime / endTime;
            transform.localScale = Vector3.Lerp(shrinkSize, expandSize, lerpRate);
            startTime += Time.deltaTime;
            yield return null;
        }

        this.enabled = false;
    }

    IEnumerator Shrink() 
    {
        Vector3 expandSize = transform.localScale;
        Vector3 shrinkSize = new Vector3(0.1f, 0.1f, 0.1f);

        float endTime = 0.21f;
        float startTime = 0.1f;
        
        while (startTime <= endTime)
        {
            float lerpRate = startTime / endTime;
            transform.localScale = Vector3.Lerp(expandSize, shrinkSize, lerpRate);
            startTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(refractWait);
        StartCoroutine(Expand());
    }
}
