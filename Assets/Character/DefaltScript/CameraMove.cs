using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject Target;

    public GameObject Character;

    private void Update() {
            // 타겟과 캐릭터 사이의 연장선을 계산
            Vector3 extensionLine = Target.transform.position - Character.transform.position;


            Vector3 cameraPosition = Character.transform.position - extensionLine.normalized * 150f;

            cameraPosition.y+=60f;

            // 카메라 시선 설정 (타겟을 바라보게 함)
            transform.LookAt(Target.transform.position);

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, cameraPosition, 0.02f);

            // 카메라 위치 업데이트
            transform.position = smoothedPosition;
    }
}
