using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class TeleportStraight : MonoBehaviour
{
    //텔레포트를 표시할 ui
    public Transform teleportCircleUI;
    //선을 그릴 라인 렌더러
    LineRenderer lr;

    //워프 사용 여부
    public bool isWarp = false;
    //워프에 걸리는 시간
    public float warpTime = 0.1f;
    //사용하고 있는 포스트 프로세싱 볼륨 컴포넌트
    public PostProcessVolume post;

    // Start is called before the first frame update
    void Start()
    {
        //시작할 떄 비활성화
        teleportCircleUI.gameObject.SetActive(false);
        //라인 렌더러 컴포넌트 얻어오기
        lr=GetComponent<LineRenderer>();
    }
    Vector3 originScale= Vector3.one*0.02f;
    // Update is called once per frame
    void Update()
    {
        Vector3 leftHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Quaternion leftHandRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);

        Vector3 lHandDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward;
        lHandDirection=Camera.main.transform.TransformDirection(lHandDirection);
        //
        if (Input.GetButtonDown("Fire1") || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch)){
            //라인 렌더러 컴포넌트 활성화
            lr.enabled = true;
        }

        //떼었을 때
        else if (Input.GetButtonUp("Fire1") || OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.LTouch)){
            lr.enabled = false;
            if (teleportCircleUI.gameObject.activeSelf)
            {
                if (isWarp == false)
                {
                    GetComponent<CharacterController>().enabled = false;
                    //텔레포트 UI위치로 순간 이동
                    transform.position = teleportCircleUI.position + Vector3.up;
                    GetComponent<CharacterController>().enabled = true;
                }
                else
                {
                    //워프 기능을 사용할 때는 Warp() 코루틴 호출
                    StartCoroutine(Warp());
                }
            }
            teleportCircleUI.gameObject.SetActive(false);
        }


        //누르고 있을 때
        else if (Input.GetButton("Fire1") || OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            Ray ray = new Ray(Camera.main.transform.TransformPoint(leftHandPosition),
                              Camera.main.transform.TransformDirection(leftHandRotation * Vector3.forward));
            RaycastHit hitInfo;
            int layer = 1 << LayerMask.NameToLayer("Terrain");
            //2.Terrain만 Ray 충돌 검출
            if (Physics.Raycast(ray, out hitInfo, 200,layer)) {
                // 부딪힌 지점에 텔레포트 UI 표시
                // 3. Ray가 부딪힌 지점에 라인 그리기
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hitInfo.point);

                // 4. Ray가 부딪힌 지점에 텔레포트 UI 표시
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = hitInfo.point;
                // 텔레포트 UI가 위로 누워 있도록 방향 설정
                teleportCircleUI.forward = hitInfo.normal;
                // 텔레포트 UI의 크기가 거리에 따라 보정되도록 설정
                teleportCircleUI.localScale = originScale * Mathf.Max(1, hitInfo.distance);
            }
            else
            {
                //Ray충돌이 발생하지 않으면 선이 Ray방향으로 그려지도록 처리
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hitInfo.point + lHandDirection * 200) ;
                //텔레포트 UI는 화면에서 비활성화
                teleportCircleUI.gameObject.SetActive(false);
            }
        }

    }
    IEnumerator Warp()
    {
        //워프 느낌을 표현할 모션블러
        MotionBlur blur;
        //워프 시작점 기억
        Vector3 pos=transform .position;
        //목적지
        Vector3 targetPos=teleportCircleUI.position+Vector3.up;
        //워프 경과 시간
        float currentTime = 0;
        //포스트 프로세싱에서 사용 중인 프로파일에서 모션블러 얻어오기
        post.profile.TryGetSettings<MotionBlur>(out blur);
        //워프시작전 블러 켜기
        blur.active = true;
        GetComponent<CharacterController>().enabled = false;

        //경과 시간이 워프보다 짧은 시간 동안 이동 처리
        while (currentTime < warpTime)
        {
            //경과 시간 흐르게 하기
            currentTime+= Time.deltaTime;
            //워프의 시작점에서 도착점에 도착하기 위해 워프 시간 동안 이동
            transform.position = Vector3.Lerp(pos, targetPos, currentTime / warpTime);
            //코루틴 대기
            yield return null;
        }
        //텔레포트 UI 위치로 순간이동
        transform.position = teleportCircleUI.position + Vector3.up;
        //캐릭터 컨트롤러 다시 켜기
        GetComponent<CharacterController>().enabled = true;
        //포스트 효과 끄기
        blur.active = false;

    }
}
