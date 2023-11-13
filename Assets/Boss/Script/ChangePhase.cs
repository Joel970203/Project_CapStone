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

    Boss_Info boss;
    Animator anim;

    void Start() {
        boss = this.GetComponent<Boss_Info>();
        bossHealth = boss.HP;
        phase2Health = bossHealth * 3 / 4;
        phase3Health = bossHealth * 1 / 2;
        phase4Health = bossHealth * 1 / 4;

        anim = GetComponent<Animator>();
        Debug.Log(bossHealth);
        Debug.Log(phase2Health);
        Debug.Log(phase3Health);
        Debug.Log(phase4Health);
    }

    // Update is called once per frame
    void Update()
    { 
        float currentHealth = boss.HP;

        if (currentHealth <= phase2Health && currentHealth > phase3Health) 
        {   
            if(!phase2HasExecuted) 
            {
                anim.SetTrigger("Phase2");
                boss.SetPhaseNum(2);
                phase2HasExecuted = true;
            }
        }    
        else if (currentHealth <= phase3Health && currentHealth > phase4Health) 
        {
            if(!phase3HasExecuted) {
                anim.SetTrigger("Phase3");
                boss.SetPhaseNum(3);
                phase3HasExecuted = true;
            }
        }    
        else if (currentHealth <= phase4Health) 
        {
            if(!phase4HasExecuted) {
                anim.SetTrigger("Phase4");
                boss.SetPhaseNum(4);
                phase4HasExecuted = true;
            }
        }   
    }
}
