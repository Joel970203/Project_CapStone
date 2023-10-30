using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject Target;
    public Vector3 RelativePosition;

    private void Start() {
        RelativePosition = this.transform.position-Target.transform.position;
    }

    private void Update() {
        this.transform.position=Target.transform.position+RelativePosition;
    }
}
