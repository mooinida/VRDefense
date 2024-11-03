using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform cameraRig; // OVRCameraRig의 Transform을 참조합니다.
    public Transform centerEyeAnchor; // OVRCameraRig의 CenterEyeAnchor를 참조합니다.
    public float rotationSpeed = 5f; // 회전 속도 조절

    void Update()
    {
        // CenterEyeAnchor의 회전 값을 가져옵니다.
        Quaternion headRotation = centerEyeAnchor.rotation;

        // 회전값을 Player 오브젝트에 적용합니다.
        cameraRig.rotation = Quaternion.Slerp(cameraRig.rotation, headRotation, Time.deltaTime * rotationSpeed);
    }
}
