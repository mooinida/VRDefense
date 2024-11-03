using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grab : MonoBehaviour
{
    bool isGrabbing = true;
    GameObject grabbedObject;
    public LayerMask grabbedLayer;
    //public float grabRange = 0.2f;
    public bool isRemoteGrab = true;
    public float remoteGrabDistance = 10;
    LineRenderer lr;
    public Transform crosshairUI;
    Vector3 originScale = Vector3.one * 0.02f;
    //���� ȸ��
    Quaternion prevRot;
    //ȸ����
    public float rotPower = 5;
    
    
    static Transform rHand;


    //���� ��ġ
    Vector3 prevPos;
    //���� ��
    float throwPower = 10;

    public static Transform RHand
    {
        get
        {
            if (rHand == null)
            {
                GameObject handObj = new GameObject("RHand");
                rHand = handObj.transform;
                //rHand.parent = Camera.main.transform;
                rHand.parent = OVRManager.instance.transform.Find("TrackingSpace/RightHandAnchor");
            }
            return rHand;
        }
    }

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        crosshairUI.gameObject.SetActive(false);
    }

    void Update()
    {
        Vector3 RHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion RHandRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        Vector3 RHandDirection = Camera.main.transform.TransformDirection(RHandRotation * Vector3.forward);

        // 1. Button Press: Activate LineRenderer
        if (Input.GetButtonDown("Fire2") || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            lr.enabled = true;
        }
        // 2. Button Held: Cast Ray and Check for Bullet Layer
        else if (Input.GetButton("Fire2") || OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            Ray ray = new Ray(Camera.main.transform.TransformPoint(RHandPosition), RHandDirection);
            RaycastHit hitInfo;

            // Cast a ray and check for objects on the "Bullet" layer
            if (Physics.Raycast(ray, out hitInfo, remoteGrabDistance, grabbedLayer))
            {
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hitInfo.point);

                // If it hits, show and enlarge the crosshair
                crosshairUI.gameObject.SetActive(true);
                crosshairUI.position = hitInfo.point;
                crosshairUI.forward = hitInfo.normal;
                crosshairUI.localScale = originScale * Mathf.Max(1, hitInfo.distance * 2); // Scale up when hit
            }
            else
            {
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, ray.origin + RHandDirection * 200);
                crosshairUI.gameObject.SetActive(false);
            }
        }
        // 3. Button Release: Try to Grab Object if Crosshair is Active
        else if (Input.GetButtonUp("Fire2") || OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            if (crosshairUI.gameObject.activeSelf&& isGrabbing)
            {
                // If crosshair is active (meaning an object was hit), attempt to grab
                TryGrab();
                //isGrabbing = false;

            }
            else if ( !isGrabbing)
            {
                TryUngrab();
            }

            // Disable line and crosshair
            lr.enabled = false;
            crosshairUI.gameObject.SetActive(false);
        }
    }

    private void TryGrab()
    {
        Vector3 RHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion RHandRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        Vector3 RHandDirection = Camera.main.transform.TransformDirection(RHandRotation * Vector3.forward);
        Vector3 playerPosition = Camera.main.transform.position;
        Vector3 playerForward = Camera.main.transform.forward;
        if (isRemoteGrab && isGrabbing)
        {
            Ray ray = new Ray(Camera.main.transform.TransformPoint(RHandPosition), RHandDirection);
            RaycastHit hitInfo;

            // Attempt to grab if the Ray hits an object on the "Bomb" layer
            if (Physics.SphereCast(ray, 0.5f, out hitInfo, remoteGrabDistance, grabbedLayer))
            {
                //isGrabbing = true;
                grabbedObject = hitInfo.transform.gameObject;
                /*float distanceFromPlayer = 1.0f;


                
                // Rotate the forward direction by 10 degrees
                Quaternion offsetRotation = Quaternion.AngleAxis(10, Camera.main.transform.up);
                Vector3 adjustedDirection = offsetRotation * playerForward;

                // Position the object 1 unit away in the adjusted direction
                grabbedObject.transform.position = playerPosition + adjustedDirection * distanceFromPlayer;
                grabbedObject.transform.rotation = Quaternion.LookRotation(adjustedDirection);
               

                grabbedObject.transform.parent = RHand;
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                */
                StartCoroutine(ApproachPlayer(grabbedObject));


                Debug.Log("Successfully grabbed: " + grabbedObject.name);
                Debug.Log(grabbedObject.transform.position);
                OVRInput.SetControllerVibration(1.0f, 1.0f, OVRInput.Controller.RTouch);
                //�ʱ� ��ġ �� ����
                prevPos = RHandPosition;
                //�ʱ� ȸ�� �� ����
                prevRot = RHandRotation;
            }
        }


        isGrabbing = false;//���⼭ false�� ��ȯ�ϴ� ���� ���̻� ������ ���� �� ���� ���·� ����� ����.
    }


   

    private void TryUngrab()
    {
        // ���� �� ��ġ ��������
        Vector3 RHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        // ���� �ӵ� ��� (���� �̵� ���� �� �ӵ��� ������� ������)
        Vector3 throwDirection = (RHandPosition - prevPos) / Time.deltaTime;

        // ���� ��ġ�� ���� �������� ���� ���� ��ġ�� ����
        prevPos = RHandPosition;
        // ���ʹϿ� ����
        // angle1=Q1, angle2=Q2
        //angle1+angle2=Q1*Q2
        //-angle=Quternion.Inverse(Q2)
        //angle2-angle1=Quaternion.FromToRotation(Q1,Q2)=Q2*Quaternion.Inverse(Q1)
        //ȸ������ = current-previous�� ���� ����. -previous�� Inverse�� ����
        Quaternion deltaRotation = RHand.rotation * Quaternion.Inverse(prevRot);
        //���� ȸ�� ����
        prevRot = RHand.rotation;

        // ��� �ִ� ��ü�� �ִٸ�
        if (grabbedObject != null)
        {
            // ���� ��� Ȱ��ȭ
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            // �տ��� ��ü �����
            grabbedObject.transform.parent = null;

            // ���� �ӵ��� ������ ���� �����Ͽ� ������ �ӵ� ����
            grabbedObject.GetComponent<Rigidbody>().velocity = throwDirection * throwPower;

            //���ӵ� = (1/dt) * d��Ÿ(Ư�� �� ���� ���� ����)
            float angle;
            Vector3 axis;
            deltaRotation.ToAngleAxis(out angle, out axis);
            Vector3 angularVelocity = (1.0f / Time.deltaTime) * angle * axis;
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = angularVelocity;

            // ���� ��ü �ʱ�ȭ
            grabbedObject = null;
            isGrabbing = true;
            OVRInput.SetControllerVibration(1,1, OVRInput.Controller.RTouch);
        }
    }

    private IEnumerator ApproachPlayer(GameObject objectToMove)
    {
        float duration = 1f; // ��ü�� �ٰ����� �� �ɸ��� �ð� (�� ����)
        float elapsed = 0f;

        // ���� ��ġ�� ��ǥ ��ġ ����
        Vector3 startPosition = objectToMove.transform.position;
        Vector3 playerPosition = Camera.main.transform.position;
        Vector3 playerForward = Camera.main.transform.forward;

        // ��ǥ ��ġ�� �÷��̾� ���� 1 ����, 10�� ������ ����
        float distanceFromPlayer = 1.0f;
        Quaternion offsetRotation = Quaternion.AngleAxis(10, Camera.main.transform.up);
        Vector3 adjustedDirection = offsetRotation * playerForward;
        Vector3 targetPosition = playerPosition + adjustedDirection * distanceFromPlayer;

        while (elapsed < duration)
        {
            // ��� �ð� ������Ʈ
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // 0���� 1�� ����� ���

            // ��ü�� ��ǥ ��ġ�� �ε巴�� �̵�
            objectToMove.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        // �ڷ�ƾ�� ������ �� ��Ȯ�� ��ǥ ��ġ�� ����
        objectToMove.transform.position = targetPosition;
        objectToMove.transform.rotation = Quaternion.LookRotation(adjustedDirection);
        objectToMove.transform.parent = RHand;
    }
    private IEnumerator GrabDistance()
    {
        float duration = 1f; // ��ü�� �ٰ����� �� �ɸ��� �ð� (�� ����)
        float elapsed = 0f;
        Vector3 playerPosition = Camera.main.transform.position;
        Vector3 playerForward = Camera.main.transform.forward;
        float distanceFromPlayer = 10f;


        grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
        // ���� ��ġ�� ��ǥ ��ġ ����
        Vector3 startPosition = grabbedObject.transform.position;
        //��ǥ ��ġ�� ����
        Vector3 targetPosition = playerPosition + playerForward * distanceFromPlayer;

        // Rotate the forward direction by 10 degrees
        Quaternion offsetRotation = Quaternion.AngleAxis(10, Camera.main.transform.up);
        Vector3 adjustedDirection = offsetRotation * playerForward;

        // Position the object 1 unit away in the adjusted direction
        grabbedObject.transform.position = playerPosition + adjustedDirection * distanceFromPlayer + Vector3.up * 10.0f;
        grabbedObject.transform.rotation = Quaternion.LookRotation(adjustedDirection);

        while (elapsed < duration)
        {
            // ��� �ð� ������Ʈ
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // 0���� 1�� ����� ���

            // ��ü�� ��ǥ ��ġ�� �ε巴�� �̵�
            grabbedObject.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }
        
        // �ڷ�ƾ�� ������ �� ��Ȯ�� ��ǥ ��ġ�� ����
        grabbedObject.transform.position = targetPosition;
        grabbedObject.transform.parent = RHand;
        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
    }

}
