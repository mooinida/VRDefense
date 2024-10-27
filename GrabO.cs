#define PC
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class GrabO: MonoBehaviour
{
    //필요 속성: 물체를 잡고 있는지 여부, 잡고 있는 물체, 잡을 물체의 종류, 잡을 수 있는 거리
    //물체를 잡고 있는지의 여부
    bool isGrabbing = false;
    //잡고 있는 물체
    GameObject grabbedObject;
    //잡을 물체의 종류
    public LayerMask grabbedLayer;
    //잡을 수 있는 거리
    public float grabRange = 0.2f;
    // 이전 위치
    Vector3 prevPos;
    // 던질 힘
    float throwPower = 10;
    // 이전 회전
    Quaternion prevRot;
    // 회전력
    public float rotPower = 5;
    // 원거리에서 물체를 잡는 기능 활성화 여부
    public bool isRemoteGrab = true;
    // 원거리에서 물체를 잡을 수 있는 거리
    public float remoteGrabDistance = 20;
    //선을 그릴 라인 렌더러
    LineRenderer lr;
    //물체를 잡을 수 있는 범위를 나타내는 ui
    public Transform grabUI;
    Vector3 originScale = Vector3.one * 0.02f;

    static Transform rHand;
    // 씬에 등록된 오른쪽 컨트롤러 찾아 반환
    public static Transform RHand
    {
        get
        {
            // 만약 rHand에 값이 없을경우
            if (rHand == null)
            {
                // RHand 이름으로 게임 오브젝트를 만든다.
                GameObject handObj = new GameObject("RHand");
                // 만들어진 객체의 트렌스폼을 rHand에 할당
                rHand = handObj.transform;
                // 컨트롤러를 카메라의 자식 객체로 등록
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
        //물체 잡기
        //1. 물체를 잡지 않고 있을 경우
        if (isGrabbing == false)
        {
            //잡기 시도
            TryGrab();
        }
    }
    private void TryGrab()
    {

        Vector3 RHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion RHandRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        Vector3 RHandDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
        RHandDirection = Camera.main.transform.TransformDirection(RHandDirection);


        //grab 버튼을 누르면 일정 영역 안에 있는 폭탄을 잡는다.
        //1. [Grab]버튼을 눌렀다면
        if (Input.GetButtonDown("Fire2") || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            lr.enabled = true;
            //원거리 물체 잡기를 사용한다면
            if (isRemoteGrab)
            {
                //손 방향으로 Ray 제작
                Ray ray = new Ray(Camera.main.transform.TransformPoint(RHandPosition),
                              Camera.main.transform.TransformDirection(RHandDirection));
                RaycastHit hitInfo;

                //SphereCast를 이용해 물체 충돌을 체크
                if (Physics.SphereCast(ray, 0.5f, out hitInfo, remoteGrabDistance, grabbedLayer))
                {

                    //Ray가 부딪힌 지점에 라인 그리기
                    lr.SetPosition(0, ray.origin);
                    lr.SetPosition(1, hitInfo.point);

                    //Ray가 부딪힌 지점에 UI 표시
                    grabUI.gameObject.SetActive(true);
                    grabUI.position = hitInfo.point;
                    grabUI.forward = hitInfo.normal;
                    //크기가 거리에 따라 보정되도록 설정
                    grabUI.localScale = originScale * Mathf.Max(1, hitInfo.distance);

                    //잡은 상태로 전환
                    isGrabbing = true;
                    //잡은 물체에 대한 기억
                    grabbedObject = hitInfo.transform.gameObject;
                    //물체가 끌려오는 기능 실행
                    //Couroutine 함수 예정
                    Debug.Log("Succesfully Grabbed: " + grabbedObject.name);
                    OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
                }
                else
                {

                    //Ray충돌이 발생하지 않으면 선이 Ray방향으로 그려지도록 처리
                    lr.SetPosition(0, ray.origin);
                    lr.SetPosition(1, hitInfo.point + RHandDirection * 200);
                    //텔레포트 UI는 화면에서 비활성화
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

            //2. 일정 영역 안에 폭탄이 있을 때
            //영역 안에 있는 모든 폭탄 검출
            Collider[] hitObjects = Physics.OverlapSphere(RHandPosition, grabRange, grabbedLayer);

            //가장 가까운  폭탄 인덱스
            int closest = 0;
            //손과 가장 가까운 물체 선택
            for (int i = 1; i < hitObjects.Length; i++)
            {
                //손과 가장 가까운 물체와의 거리
                Vector3 closestPos = hitObjects[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos, RHandPosition);
                //다음 물체와 손의 거리
                Vector3 nextPos = hitObjects[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos, RHandPosition);
                //다음 물체와의 거리가 더 가깝다면
                if (nextDistance < closestDistance)
                {
                    closest = i;
                }
            }
            //3.폭탄을 잡는다.
            //검출된 물체가 있을 경우
            if (hitObjects.Length > 0)
            {
                //잡은 상태로 전환
                isGrabbing = true;
                //잡은 물체에 대한 기억
                grabbedObject = hitObjects[closest].gameObject;
                //잡은 물체를 손의 자식으로 등록
                grabbedObject.transform.parent = RHand;

                // 물리 기능 정지
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }

    }

}
