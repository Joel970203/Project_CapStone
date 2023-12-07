using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavBulletDamageHandler : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] float customY;
    [SerializeField] GameObject effect;

    private void OnParticleCollision(GameObject other)  
    {
        if (other.CompareTag("Player"))
        {
            Vector3 targetPoint = transform.position;
            targetPoint.y = customY;
            GameObject targetEffect = Instantiate(effect, targetPoint, Quaternion.identity);
            Destroy(targetEffect, 3f);
            Destroy(gameObject);
        }
    }
}
