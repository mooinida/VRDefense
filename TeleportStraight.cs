using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class TeleportStraight : MonoBehaviour
{
    //�ڷ���Ʈ�� ǥ���� ui
    public Transform teleportCircleUI;
    //���� �׸� ���� ������
    LineRenderer lr;

    //���� ��� ����
    public bool isWarp = false;
    //������ �ɸ��� �ð�
    public float warpTime = 0.1f;
    //����ϰ� �ִ� ����Ʈ ���μ��� ���� ������Ʈ
    public PostProcessVolume post;

    // Start is called before the first frame update
    void Start()
    {
        //������ �� ��Ȱ��ȭ
        teleportCircleUI.gameObject.SetActive(false);
        //���� ������ ������Ʈ ������
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
            //���� ������ ������Ʈ Ȱ��ȭ
            lr.enabled = true;
        }

        //������ ��
        else if (Input.GetButtonUp("Fire1") || OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.LTouch)){
            lr.enabled = false;
            if (teleportCircleUI.gameObject.activeSelf)
            {
                if (isWarp == false)
                {
                    GetComponent<CharacterController>().enabled = false;
                    //�ڷ���Ʈ UI��ġ�� ���� �̵�
                    transform.position = teleportCircleUI.position + Vector3.up;
                    GetComponent<CharacterController>().enabled = true;
                }
                else
                {
                    //���� ����� ����� ���� Warp() �ڷ�ƾ ȣ��
                    StartCoroutine(Warp());
                }
            }
            teleportCircleUI.gameObject.SetActive(false);
        }


        //������ ���� ��
        else if (Input.GetButton("Fire1") || OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            Ray ray = new Ray(Camera.main.transform.TransformPoint(leftHandPosition),
                              Camera.main.transform.TransformDirection(leftHandRotation * Vector3.forward));
            RaycastHit hitInfo;
            int layer = 1 << LayerMask.NameToLayer("Terrain");
            //2.Terrain�� Ray �浹 ����
            if (Physics.Raycast(ray, out hitInfo, 200,layer)) {
                // �ε��� ������ �ڷ���Ʈ UI ǥ��
                // 3. Ray�� �ε��� ������ ���� �׸���
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hitInfo.point);

                // 4. Ray�� �ε��� ������ �ڷ���Ʈ UI ǥ��
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = hitInfo.point;
                // �ڷ���Ʈ UI�� ���� ���� �ֵ��� ���� ����
                teleportCircleUI.forward = hitInfo.normal;
                // �ڷ���Ʈ UI�� ũ�Ⱑ �Ÿ��� ���� �����ǵ��� ����
                teleportCircleUI.localScale = originScale * Mathf.Max(1, hitInfo.distance);
            }
            else
            {
                //Ray�浹�� �߻����� ������ ���� Ray�������� �׷������� ó��
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hitInfo.point + lHandDirection * 200) ;
                //�ڷ���Ʈ UI�� ȭ�鿡�� ��Ȱ��ȭ
                teleportCircleUI.gameObject.SetActive(false);
            }
        }

    }
    IEnumerator Warp()
    {
        //���� ������ ǥ���� ��Ǻ�
        MotionBlur blur;
        //���� ������ ���
        Vector3 pos=transform .position;
        //������
        Vector3 targetPos=teleportCircleUI.position+Vector3.up;
        //���� ��� �ð�
        float currentTime = 0;
        //����Ʈ ���μ��̿��� ��� ���� �������Ͽ��� ��Ǻ� ������
        post.profile.TryGetSettings<MotionBlur>(out blur);
        //���������� �� �ѱ�
        blur.active = true;
        GetComponent<CharacterController>().enabled = false;

        //��� �ð��� �������� ª�� �ð� ���� �̵� ó��
        while (currentTime < warpTime)
        {
            //��� �ð� �帣�� �ϱ�
            currentTime+= Time.deltaTime;
            //������ ���������� �������� �����ϱ� ���� ���� �ð� ���� �̵�
            transform.position = Vector3.Lerp(pos, targetPos, currentTime / warpTime);
            //�ڷ�ƾ ���
            yield return null;
        }
        //�ڷ���Ʈ UI ��ġ�� �����̵�
        transform.position = teleportCircleUI.position + Vector3.up;
        //ĳ���� ��Ʈ�ѷ� �ٽ� �ѱ�
        GetComponent<CharacterController>().enabled = true;
        //����Ʈ ȿ�� ����
        blur.active = false;

    }
}
