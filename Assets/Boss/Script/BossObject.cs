using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossObject : MonoBehaviour
{
    //시험을 위해 싱글톤으로 임시로 구현
    public static BossObject instance;
    [SerializeField]private float maxHealth = 1000.0f;
    [SerializeField]private float currentHealth = 1000.0f;
    [SerializeField]private float currentPhase = 1.0f;

    void Awake()
    {
        if (instance == null) {
            instance = this;
        }
    }

    public float GetBossHealth() 
    {
        return maxHealth; 
    }

    public float GetBossCurrentHealth() 
    {
        return currentHealth; 
    }

    public void SetBossPhase(float phaseNum) 
    {
        currentPhase = phaseNum;
    }

    public float GetCurrentPhase() 
    {
        return currentPhase; 
    }
}
