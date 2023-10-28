using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Info : MonoBehaviour
{
    [SerializeField]
    private int HP;

    [SerializeField]
    private int AD;

    [SerializeField]
    private int armor;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void TakeDamage(int damage)
    {
        if (damage - armor > 0)
        {
            HP -= damage - armor;
            if (HP <= 0)
            {
                Death();
            }
        }
    }

    private void Death()
    {

    }
}
