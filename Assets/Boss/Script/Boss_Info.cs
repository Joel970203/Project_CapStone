using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Info : MonoBehaviour
{
    [SerializeField]
    public int HP;
    [SerializeField]
    int armor;

    public int phaseNum = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Death()
    {

    }

    public void SetPhaseNum(int num)
    {
        phaseNum = num;
    }
}
