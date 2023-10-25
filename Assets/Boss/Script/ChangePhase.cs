using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePhase : MonoBehaviour
{
    //임시 코드로 하드코딩
    //페이즈 번호를 1,2,3,4로 매기고 100~76: 페이즈1, 75~51: 페이즈2, 50~25: 페이즈3, 25~0: 페이즈4
    float bossHealth;
    float phase2Health;
    float phase3Health;
    float phase4Health;

    private bool phase2HasExecuted = false;
    private bool phase3HasExecuted = false;
    private bool phase4HasExecuted = false; 

    void Start() {
        bossHealth = BossObject.instance.GetBossHealth();
        BossObject.instance.SetBossPhase(1.0f);
        phase2Health = bossHealth * 3 / 4;
        phase3Health = bossHealth * 1 / 2;
        phase4Health = bossHealth * 1 / 4;
        Debug.Log(bossHealth);
        Debug.Log(phase2Health);
        Debug.Log(phase3Health);
        Debug.Log(phase4Health);
    }

    // Update is called once per frame
    void Update()
    { 
        float currentHealth = BossObject.instance.GetBossCurrentHealth();

        if (currentHealth <= phase2Health && currentHealth > phase3Health) 
        {   
            if(!phase2HasExecuted) 
            {
                BossObject.instance.SetBossPhase(2.0f);
                phase2HasExecuted = true;
            }
        }    
        else if (currentHealth <= phase3Health && currentHealth > phase4Health) 
        {
            if(!phase3HasExecuted) {
                BossObject.instance.SetBossPhase(3.0f);
                phase3HasExecuted = true;
            }
        }    
        else if (currentHealth <= phase4Health) 
        {
            if(!phase4HasExecuted) {
                BossObject.instance.SetBossPhase(4.0f);
                phase4HasExecuted = true;
            }
        }   
    }
}
