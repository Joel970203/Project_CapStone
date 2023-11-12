// Grenade 스크립트
using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float explosionRadius = 10.0f;
    [SerializeField] private float explosionForce = 700.0f;
    [SerializeField] private float throwForce = 3.0f;

    private Rigidbody rb;
    private bool hasExploded = false; // 폭발 여부를 나타내는 플래그

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce);
        rb.angularVelocity = new Vector3(0, 0, 90);
        // 코루틴을 통해 폭발 처리
        StartCoroutine(ExplodeAfterDelay(1.0f));
    }

    private IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!hasExploded)
        {
            // 폭발 이펙트 생성
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, transform.rotation);

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider hit in colliders)
            {
                Rigidbody targetRigidbody = hit.GetComponent<Rigidbody>();
                if (targetRigidbody != null)
                {
                    targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }

            // 수류탄 오브젝트 제거
            Destroy(gameObject);

            // 폭발 이펙트도 자체적으로 제거될 수 있도록 설정
            Destroy(explosionEffect, explosionEffect.GetComponent<ParticleSystem>().main.duration);

            hasExploded = true; // 폭발한 상태로 플래그 설정
        }
    }
}
