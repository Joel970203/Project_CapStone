using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove : MonoBehaviour
{
    private Animator anim;
    private bool anim1HasExecuted;
    private bool anim2HasExecuted;
    private bool anim3HasExecuted;
    private bool anim4HasExecuted;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("IdleState", 0.0f);
        anim1HasExecuted = false;
        anim2HasExecuted = false;
        anim3HasExecuted = false;
        anim4HasExecuted = false;
    }

    // Update is called once per frame
    void Update()
    {
        float currentPhase = BossObject.instance.GetCurrentPhase();
        //Debug.Log(currentPhase);

        switch(currentPhase) {
            case 1.0f:
                if(!anim1HasExecuted) 
                {
                    Debug.Log("a1");
                    anim.SetFloat("IdleState", 1.0f);
                    anim1HasExecuted = true;
                }
                break;
            case 2.0f:
                if(!anim2HasExecuted) 
                {
                    Debug.Log("a2");
                    anim.SetFloat("IdleState", 2.0f);
                    anim2HasExecuted = true;
                }
                break;
            case 3.0f:
                if(!anim3HasExecuted) 
                {
                    Debug.Log("a3");
                    anim.SetFloat("IdleState", 3.0f);
                    anim3HasExecuted = true;
                }
                break;
            case 4.0f:
                if(!anim4HasExecuted) 
                {
                    Debug.Log("a4");
                    anim.SetFloat("IdleState", 4.0f);
                    anim4HasExecuted = true;
                }
                break;
            default:
                break;
        }
    }
}
