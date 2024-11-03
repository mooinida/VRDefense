using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    //랜덤 시간의 범위
    public float minTime = 1;
    public float maxTime = 5;
    //생성 시간
    float createTime;
    //경과 시간
    float currentTime;
    //드론을 생성할 위치
    public Transform[] spawnPoints;
    //드론 공장
    public GameObject droneFactory;
    void Start()
    {
        StartCoroutine(SpawnDrone());
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    IEnumerator SpawnDrone()
    {
        float createTIme = Random.Range(minTime, maxTime);

        yield return new WaitForSeconds(createTIme);

        GameObject drone = Instantiate(droneFactory);

        // 드론 위치를 랜덤으로 설정합니다.
        int index = Random.Range(0, spawnPoints.Length);
        drone.transform.position = spawnPoints[index].position;
    }
}

