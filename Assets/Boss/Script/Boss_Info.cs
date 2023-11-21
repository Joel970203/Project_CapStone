using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss_Info : MonoBehaviour
{
    [SerializeField] private int Max_HP;
    public int HP;
    [SerializeField] private int armor;

    private int phaseNum = 1;
    // Start is called before the first frame update
    void Start()
    {
        //HP = Max_HP;
        StartCoroutine(CheckPhase2());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Death()
    {

    }

    IEnumerator CheckPhase2()
    {   
        while((float)HP / Max_HP > 0.99f) yield return null;

        phaseNum = 2;
        Debug.Log("2");
        StartCoroutine(CheckPhase3());
    }

    IEnumerator CheckPhase3()
    {   
        while((float)HP / Max_HP > 0.3f) yield return null;

        phaseNum = 3;
        Debug.Log("3");
        StartCoroutine(CheckPhase4());
    }

    IEnumerator CheckPhase4()
    {   
        while(HP > 0) yield return null;

        Death();
    }

    public void SetPhaseNum(int num)
    {
        phaseNum = num;
    }

    public int GetPhaseNum()
    {
        return phaseNum;
    }
}
