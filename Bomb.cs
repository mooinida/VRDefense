using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    //폭발 효과
    Transform explosion;
    ParticleSystem expEffect;
    AudioSource expAudio;
    //폭발 영역
    public float range = 5;

    void Start()
    {
        //씬에서 Explosion 객체를 찾아 transform 가져오기
        explosion = GameObject.Find("Explosion").transform;
        //Explosion 객체의 ParticleSystem 컴포넌트 얻어오기
        expEffect=explosion.GetComponent<ParticleSystem>();
        //Explosion 객체의 오디오소스 컴포넌트 가져오기
        expAudio=explosion.GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }
}
