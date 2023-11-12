using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Particle : MonoBehaviour
{
    public TrailRenderer trailRendererPrefab; // Trail Renderer 프리팹을 Inspector에서 할당
    private TrailRenderer trailRenderer; // 생성된 Trail Renderer 인스턴스

    private void Start()
    {
        // Trail Renderer를 인스턴스화
        if (trailRendererPrefab != null)
        {
            trailRenderer = Instantiate(trailRendererPrefab, transform.position, Quaternion.identity);
            trailRenderer.emitting = false; // 시작 시에는 트레일 미발생
        }
    }

    private void Update()
    {
        // Arrow가 이동할 때 Trail Renderer를 활성화
        if (trailRenderer != null)
        {
            trailRenderer.transform.position = transform.position; // Arrow 위치에 Trail Renderer 위치 맞춤

            // Arrow가 이동할 때마다 Trail Renderer를 활성화
            if (GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
            {
                trailRenderer.emitting = true;
            }
            else
            {
                trailRenderer.emitting = false;
            }
        }
    }
}



