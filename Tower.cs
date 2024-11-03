using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    //������ ǥ���� UI
    public Transform damageUI;
    public Image damageImage;

    //Ÿ���� ���� HP
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
    //���� �Ÿ��� �ð�
    public float damageTime = 0.1f;
    //������ ó���� ���� �ڷ�ƾ �Լ�
    IEnumerator DamageEvent()
    {
        //damageImage ������Ʈ Ȱ��ȭ
        damageImage.enabled = true;
        //damageTime��ŭ ��ٸ���
        yield return new WaitForSeconds(damageTime);
        //�ٽ� ������� ��Ȱ��ȭ�Ѵ�.
        damageImage.enabled = false;
        Debug.Log("attacked!");
    }

    //Tower�� �̱��� ��ü
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
