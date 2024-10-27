#define PC
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class GrabO: MonoBehaviour
{
    //�ʿ� �Ӽ�: ��ü�� ��� �ִ��� ����, ��� �ִ� ��ü, ���� ��ü�� ����, ���� �� �ִ� �Ÿ�
    //��ü�� ��� �ִ����� ����
    bool isGrabbing = false;
    //��� �ִ� ��ü
    GameObject grabbedObject;
    //���� ��ü�� ����
    public LayerMask grabbedLayer;
    //���� �� �ִ� �Ÿ�
    public float grabRange = 0.2f;
    // ���� ��ġ
    Vector3 prevPos;
    // ���� ��
    float throwPower = 10;
    // ���� ȸ��
    Quaternion prevRot;
    // ȸ����
    public float rotPower = 5;
    // ���Ÿ����� ��ü�� ��� ��� Ȱ��ȭ ����
    public bool isRemoteGrab = true;
    // ���Ÿ����� ��ü�� ���� �� �ִ� �Ÿ�
    public float remoteGrabDistance = 20;
    //���� �׸� ���� ������
    LineRenderer lr;
    //��ü�� ���� �� �ִ� ������ ��Ÿ���� ui
    public Transform grabUI;
    Vector3 originScale = Vector3.one * 0.02f;

    static Transform rHand;
    // ���� ��ϵ� ������ ��Ʈ�ѷ� ã�� ��ȯ
    public static Transform RHand
    {
        get
        {
            // ���� rHand�� ���� �������
            if (rHand == null)
            {
                // RHand �̸����� ���� ������Ʈ�� �����.
                GameObject handObj = new GameObject("RHand");
                // ������� ��ü�� Ʈ�������� rHand�� �Ҵ�
                rHand = handObj.transform;
                // ��Ʈ�ѷ��� ī�޶��� �ڽ� ��ü�� ���
                rHand.parent = Camera.main.transform;

            }
            return rHand;
        }
    }


    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 RHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion RHandRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        Vector3 RHandDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
        RHandDirection = Camera.main.transform.TransformDirection(RHandDirection);
        if (Input.GetButtonDown("Fire2") || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            lr.enabled = true;
        }
        else if (Input.GetButton("Fire2") || OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {

        }
        else if (Input.GetButtonUp("Fire2") || OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {

        }
        //��ü ���
        //1. ��ü�� ���� �ʰ� ���� ���
        if (isGrabbing == false)
        {
            //��� �õ�
            TryGrab();
        }
    }
    private void TryGrab()
    {

        Vector3 RHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion RHandRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        Vector3 RHandDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
        RHandDirection = Camera.main.transform.TransformDirection(RHandDirection);


        //grab ��ư�� ������ ���� ���� �ȿ� �ִ� ��ź�� ��´�.
        //1. [Grab]��ư�� �����ٸ�
        if (Input.GetButtonDown("Fire2") || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            lr.enabled = true;
            //���Ÿ� ��ü ��⸦ ����Ѵٸ�
            if (isRemoteGrab)
            {
                //�� �������� Ray ����
                Ray ray = new Ray(Camera.main.transform.TransformPoint(RHandPosition),
                              Camera.main.transform.TransformDirection(RHandDirection));
                RaycastHit hitInfo;

                //SphereCast�� �̿��� ��ü �浹�� üũ
                if (Physics.SphereCast(ray, 0.5f, out hitInfo, remoteGrabDistance, grabbedLayer))
                {

                    //Ray�� �ε��� ������ ���� �׸���
                    lr.SetPosition(0, ray.origin);
                    lr.SetPosition(1, hitInfo.point);

                    //Ray�� �ε��� ������ UI ǥ��
                    grabUI.gameObject.SetActive(true);
                    grabUI.position = hitInfo.point;
                    grabUI.forward = hitInfo.normal;
                    //ũ�Ⱑ �Ÿ��� ���� �����ǵ��� ����
                    grabUI.localScale = originScale * Mathf.Max(1, hitInfo.distance);

                    //���� ���·� ��ȯ
                    isGrabbing = true;
                    //���� ��ü�� ���� ���
                    grabbedObject = hitInfo.transform.gameObject;
                    //��ü�� �������� ��� ����
                    //Couroutine �Լ� ����
                    Debug.Log("Succesfully Grabbed: " + grabbedObject.name);
                    OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
                }
                else
                {

                    //Ray�浹�� �߻����� ������ ���� Ray�������� �׷������� ó��
                    lr.SetPosition(0, ray.origin);
                    lr.SetPosition(1, hitInfo.point + RHandDirection * 200);
                    //�ڷ���Ʈ UI�� ȭ�鿡�� ��Ȱ��ȭ
                    grabUI.gameObject.SetActive(false);

                    Debug.Log("Nothing grabbed");
                    //OVRInput.SetControllerVibration(2, 2, OVRInput.Controller.RTouch);
                }
            }
            if (Input.GetButton("Fire2") || OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch))
            {
                lr.enabled = false;
            }
            if (Input.GetButtonUp("Fire2") || OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch))
            {
                lr.enabled = false;
            }

            //2. ���� ���� �ȿ� ��ź�� ���� ��
            //���� �ȿ� �ִ� ��� ��ź ����
            Collider[] hitObjects = Physics.OverlapSphere(RHandPosition, grabRange, grabbedLayer);

            //���� �����  ��ź �ε���
            int closest = 0;
            //�հ� ���� ����� ��ü ����
            for (int i = 1; i < hitObjects.Length; i++)
            {
                //�հ� ���� ����� ��ü���� �Ÿ�
                Vector3 closestPos = hitObjects[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos, RHandPosition);
                //���� ��ü�� ���� �Ÿ�
                Vector3 nextPos = hitObjects[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos, RHandPosition);
                //���� ��ü���� �Ÿ��� �� �����ٸ�
                if (nextDistance < closestDistance)
                {
                    closest = i;
                }
            }
            //3.��ź�� ��´�.
            //����� ��ü�� ���� ���
            if (hitObjects.Length > 0)
            {
                //���� ���·� ��ȯ
                isGrabbing = true;
                //���� ��ü�� ���� ���
                grabbedObject = hitObjects[closest].gameObject;
                //���� ��ü�� ���� �ڽ����� ���
                grabbedObject.transform.parent = RHand;

                // ���� ��� ����
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }

    }

}
