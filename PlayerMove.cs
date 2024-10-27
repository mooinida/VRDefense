using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //이동속도
    public float speed = 5;
    CharacterController cc;

    //중력 가속도의 크기
    public float gravity = -20;
    //수직속도
    float yVelocity = 0;
    //점프 크기
    public float jumpPower = 5;
    //지면에 닿앗을 때 튀어오른느 힘


    // Start is called before the first frame update
    void Start()
    {
        cc=GetComponent<CharacterController>();        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");  // 왼쪽 스틱의 X축 입력
        float v = Input.GetAxis("Vertical");  // 왼쪽 스틱의 Y축 입력
        // 2. 방향을 만든다. (스틱 입력을 이용한 방향)
        Vector3 dir = new Vector3(h, 0, v);
        dir=Camera.main.transform.TransformDirection(dir);
        //2.1 중력을 적용한 수직 방향 추가 v=v0+at
        yVelocity += gravity * Time.deltaTime;
       

        //2.2 바닥에 있을 경우 수직 항력을 처리하기 위해 속도를 0으로 한다.
        if(cc.isGrounded)
        {
            yVelocity = 0;
            //2.3 사용자가 점프 버튼을 누르면 속도에 점프 크기를 할당
            if (Input.GetButtonDown("Jump") || OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
            {
                yVelocity = jumpPower;
            }
        }
        
        
        dir.y = yVelocity;
        
        //3.이동한다.
        cc.Move(dir*speed*Time.deltaTime);
    }
}
