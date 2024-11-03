using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform cameraRig; // OVRCameraRig�� Transform�� �����մϴ�.
    public Transform centerEyeAnchor; // OVRCameraRig�� CenterEyeAnchor�� �����մϴ�.
    public float rotationSpeed = 5f; // ȸ�� �ӵ� ����

    void Update()
    {
        // CenterEyeAnchor�� ȸ�� ���� �����ɴϴ�.
        Quaternion headRotation = centerEyeAnchor.rotation;

        // ȸ������ Player ������Ʈ�� �����մϴ�.
        cameraRig.rotation = Quaternion.Slerp(cameraRig.rotation, headRotation, Time.deltaTime * rotationSpeed);
    }
}
