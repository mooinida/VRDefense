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

    void Start()
    {
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
}
