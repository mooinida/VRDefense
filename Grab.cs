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
    //이전 회전
    Quaternion prevRot;
    //회전력
    public float rotPower = 5;
    
    
    static Transform rHand;


    //이전 위치
    Vector3 prevPos;
    //던질 힘
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
                //초기 위치 값 지정
                prevPos = RHandPosition;
                //초기 회전 값 지정
                prevRot = RHandRotation;
            }
        }


        isGrabbing = false;//여기서 false로 전환하는 것은 더이상 물건을 잡을 수 없는 상태로 만들기 위함.
    }


   

    private void TryUngrab()
    {
        // 현재 손 위치 가져오기
        Vector3 RHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        // 손의 속도 계산 (손의 이동 방향 및 속도를 기반으로 던지기)
        Vector3 throwDirection = (RHandPosition - prevPos) / Time.deltaTime;

        // 현재 위치를 다음 프레임을 위한 이전 위치로 저장
        prevPos = RHandPosition;
        // 쿼터니온 공식
        // angle1=Q1, angle2=Q2
        //angle1+angle2=Q1*Q2
        //-angle=Quternion.Inverse(Q2)
        //angle2-angle1=Quaternion.FromToRotation(Q1,Q2)=Q2*Quaternion.Inverse(Q1)
        //회전방향 = current-previous의 차로 구함. -previous는 Inverse로 구함
        Quaternion deltaRotation = RHand.rotation * Quaternion.Inverse(prevRot);
        //이전 회전 저장
        prevRot = RHand.rotation;

        // 잡고 있는 물체가 있다면
        if (grabbedObject != null)
        {
            // 물리 기능 활성화
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            // 손에서 물체 떼어내기
            grabbedObject.transform.parent = null;

            // 손의 속도와 던지는 힘을 결합하여 던지기 속도 적용
            grabbedObject.GetComponent<Rigidbody>().velocity = throwDirection * throwPower;

            //각속도 = (1/dt) * d세타(특정 축 기준 변위 각도)
            float angle;
            Vector3 axis;
            deltaRotation.ToAngleAxis(out angle, out axis);
            Vector3 angularVelocity = (1.0f / Time.deltaTime) * angle * axis;
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = angularVelocity;

            // 잡은 물체 초기화
            grabbedObject = null;
            isGrabbing = true;
            OVRInput.SetControllerVibration(1,1, OVRInput.Controller.RTouch);
        }
    }

    private IEnumerator ApproachPlayer(GameObject objectToMove)
    {
        float duration = 1f; // 물체가 다가오는 데 걸리는 시간 (초 단위)
        float elapsed = 0f;

        // 시작 위치와 목표 위치 설정
        Vector3 startPosition = objectToMove.transform.position;
        Vector3 playerPosition = Camera.main.transform.position;
        Vector3 playerForward = Camera.main.transform.forward;

        // 목표 위치를 플레이어 앞쪽 1 유닛, 10도 각도로 설정
        float distanceFromPlayer = 1.0f;
        Quaternion offsetRotation = Quaternion.AngleAxis(10, Camera.main.transform.up);
        Vector3 adjustedDirection = offsetRotation * playerForward;
        Vector3 targetPosition = playerPosition + adjustedDirection * distanceFromPlayer;

        while (elapsed < duration)
        {
            // 경과 시간 업데이트
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // 0에서 1로 진행률 계산

            // 물체를 목표 위치로 부드럽게 이동
            objectToMove.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        // 코루틴이 끝났을 때 정확히 목표 위치에 설정
        objectToMove.transform.position = targetPosition;
        objectToMove.transform.rotation = Quaternion.LookRotation(adjustedDirection);
        objectToMove.transform.parent = RHand;
    }
    private IEnumerator GrabDistance()
    {
        float duration = 1f; // 물체가 다가오는 데 걸리는 시간 (초 단위)
        float elapsed = 0f;
        Vector3 playerPosition = Camera.main.transform.position;
        Vector3 playerForward = Camera.main.transform.forward;
        float distanceFromPlayer = 10f;


        grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
        // 시작 위치와 목표 위치 설정
        Vector3 startPosition = grabbedObject.transform.position;
        //목표 위치와 방향
        Vector3 targetPosition = playerPosition + playerForward * distanceFromPlayer;

        // Rotate the forward direction by 10 degrees
        Quaternion offsetRotation = Quaternion.AngleAxis(10, Camera.main.transform.up);
        Vector3 adjustedDirection = offsetRotation * playerForward;

        // Position the object 1 unit away in the adjusted direction
        grabbedObject.transform.position = playerPosition + adjustedDirection * distanceFromPlayer + Vector3.up * 10.0f;
        grabbedObject.transform.rotation = Quaternion.LookRotation(adjustedDirection);

        while (elapsed < duration)
        {
            // 경과 시간 업데이트
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // 0에서 1로 진행률 계산

            // 물체를 목표 위치로 부드럽게 이동
            grabbedObject.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }
        
        // 코루틴이 끝났을 때 정확히 목표 위치에 설정
        grabbedObject.transform.position = targetPosition;
        grabbedObject.transform.parent = RHand;
        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
    }

}
