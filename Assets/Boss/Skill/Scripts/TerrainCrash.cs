using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCrash : MonoBehaviour
{
    [SerializeField] GameObject effect;
    [SerializeField] float customY;
    [SerializeField] bool isDamage;
    [SerializeField] float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Terrain" || other.gameObject.tag == "Player")
        {
            Vector3 targetPoint = transform.position;
            targetPoint.y = customY;
            GameObject targetEffect = Instantiate(effect, targetPoint, Quaternion.identity);
            Destroy(targetEffect, 3f);
            if (isDamage && other.gameObject.tag == "Player") other.transform.GetComponent<Character_Info>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
