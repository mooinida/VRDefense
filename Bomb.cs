using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    //���� ȿ��
    Transform explosion;
    ParticleSystem expEffect;
    AudioSource expAudio;
    //���� ����
    public float range = 5;

    // ��ź �ε����� BombManager ����
    private int index;
    private BombManager bombManager;
    void Start()
    {
        // BombManager ���� ����
        bombManager = FindObjectOfType<BombManager>();
        //������ Explosion ��ü�� ã�� transform ��������
        explosion = GameObject.Find("Explosion").transform;
        //Explosion ��ü�� ParticleSystem ������Ʈ ������
        expEffect=explosion.GetComponent<ParticleSystem>();
        //Explosion ��ü�� ������ҽ� ������Ʈ ��������
        expAudio=explosion.GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    public void SetIndex(int bombIndex)
    {
        index = bombIndex;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //���̾� ����ũ ��������
        int layerMask =1<< LayerMask.NameToLayer("Drone");
        //��ź�� �߽����� range ũ���� �ݰ� �ȿ� ���� ��� �˻�
        Collider[] drones = Physics.OverlapSphere(transform.position, range,layerMask);
        foreach(Collider drone in drones)
        {
            Destroy(drone.gameObject);
        }
        explosion.position = transform.position;
        expEffect.Play();
        expAudio.Play();

        // ��ź ���� �� BombManager�� �˸�
        bombManager.SetBombUsed(index);
        Destroy(gameObject);
    }
}
