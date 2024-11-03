using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    //데미지 표현할 UI
    public Transform damageUI;
    public Image damageImage;

    //타워의 최초 HP
    public int initialHP = 1000;
    int hp = 0;
    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            //StopAllCoroutines();
            StartCoroutine(DamageEvent());
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    //깜빡 거리는 시간
    public float damageTime = 0.1f;
    //데미지 처리를 위한 코루틴 함수
    IEnumerator DamageEvent()
    {
        //damageImage 컴포넌트 활성화
        damageImage.enabled = true;
        //damageTime만큼 기다린다
        yield return new WaitForSeconds(damageTime);
        //다시 원래대로 비활성화한다.
        damageImage.enabled = false;
        Debug.Log("attacked!");
    }

    //Tower의 싱글턴 객체
    public static Tower Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
        hp = 1000;

        Transform cameraTransform = GameObject.Find("CenterEyeAnchor").transform;
        if (cameraTransform != null)
        {
            damageUI.SetParent(cameraTransform, false);

            float z = cameraTransform.GetComponent<Camera>().nearClipPlane + 0.5f;
            damageUI.localPosition = new Vector3(0, 0, z);

            damageImage.enabled = false;
        }


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
