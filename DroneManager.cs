using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    //���� �ð��� ����
    public float minTime = 1;
    public float maxTime = 5;
    //���� �ð�
    float createTime;
    //��� �ð�
    float currentTime;
    //����� ������ ��ġ
    public Transform[] spawnPoints;
    //��� ����
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

        // ��� ��ġ�� �������� �����մϴ�.
        int index = Random.Range(0, spawnPoints.Length);
        drone.transform.position = spawnPoints[index].position;
    }
}

