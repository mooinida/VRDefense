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

    // 폭탄 인덱스와 BombManager 참조
    private int index;
    private BombManager bombManager;
    void Start()
    {
        // BombManager 참조 설정
        bombManager = FindObjectOfType<BombManager>();
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

    public void SetIndex(int bombIndex)
    {
        index = bombIndex;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //레이어 마스크 가져오기
        int layerMask =1<< LayerMask.NameToLayer("Drone");
        //폭탄을 중심으로 range 크기의 반경 안에 들어온 드론 검사
        Collider[] drones = Physics.OverlapSphere(transform.position, range,layerMask);
        foreach(Collider drone in drones)
        {
            Destroy(drone.gameObject);
        }
        explosion.position = transform.position;
        expEffect.Play();
        expAudio.Play();

        // 폭탄 제거 전 BombManager에 알림
        bombManager.SetBombUsed(index);
        Destroy(gameObject);
    }
}
