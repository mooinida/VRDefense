using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //�̵��ӵ�
    public float speed = 5;
    CharacterController cc;

    //�߷� ���ӵ��� ũ��
    public float gravity = -20;
    //�����ӵ�
    float yVelocity = 0;
    //���� ũ��
    public float jumpPower = 5;
    //���鿡 ����� �� Ƣ������� ��


    // Start is called before the first frame update
    void Start()
    {
        cc=GetComponent<CharacterController>();        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");  // ���� ��ƽ�� X�� �Է�
        float v = Input.GetAxis("Vertical");  // ���� ��ƽ�� Y�� �Է�
        // 2. ������ �����. (��ƽ �Է��� �̿��� ����)
        Vector3 dir = new Vector3(h, 0, v);
        dir=Camera.main.transform.TransformDirection(dir);
        //2.1 �߷��� ������ ���� ���� �߰� v=v0+at
        yVelocity += gravity * Time.deltaTime;
       

        //2.2 �ٴڿ� ���� ��� ���� �׷��� ó���ϱ� ���� �ӵ��� 0���� �Ѵ�.
        if(cc.isGrounded)
        {
            yVelocity = 0;
            //2.3 ����ڰ� ���� ��ư�� ������ �ӵ��� ���� ũ�⸦ �Ҵ�
            if (Input.GetButtonDown("Jump") || OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
            {
                yVelocity = jumpPower;
            }
        }
        
        
        dir.y = yVelocity;
        
        //3.�̵��Ѵ�.
        cc.Move(dir*speed*Time.deltaTime);
    }
}
