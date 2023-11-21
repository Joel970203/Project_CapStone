using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSetUIActive : MonoBehaviour
{
    public void ActiveObjectTrue(GameObject target)
    {
        target.SetActive(true);
    }

    public void ActiveObjectFalse(GameObject target)
    {
        target.SetActive(false);
    }
}
