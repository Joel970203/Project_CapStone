using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivivalCharacter : MonoBehaviour
{
    public GameObject GameCharacter;

    public void rivival()
    {
        GameCharacter.SetActive(true);
        Destroy(this.gameObject);
    }
}
