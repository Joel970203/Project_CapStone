using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCollision : MonoBehaviour
{
    [SerializeField] GameObject effect;

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("충돌함" + other.gameObject.tag);
        if (other.gameObject.tag == "Terrain") 
        {
            Vector3 targetPoint = transform.position;
            targetPoint.y = 51.5f;
            Instantiate(effect, targetPoint, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
