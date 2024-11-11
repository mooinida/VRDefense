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
        while (true)
        {
            // 랜덤 생성 시간을 설정하고 기다림
            createTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(createTime);

            // 드론을 생성하고 위치를 랜덤으로 설정
            GameObject drone = Instantiate(droneFactory);
            int index = Random.Range(0, spawnPoints.Length);
            drone.transform.position = spawnPoints[index].position;
        }
    }
}

