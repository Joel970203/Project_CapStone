using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    // Start is called before the first frame update
    public float DeleteTime;
    private void Awake() 
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DeleteTime -= Time.deltaTime; // 경과 시간 누적

        if (DeleteTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
