using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    private bool[] Bombused;

    public GameObject bombfactory;

    public Transform[] bombPoints;
    void Start()
    {
        Bombused = new bool[bombPoints.Length];
        

        //ÃÊ±â ÆøÅº »ý¼º
        for (int i = 0; i < bombPoints.Length; i++)
        {
            Bombused[i] = true;
        }
    }

    void Update()
    {
        StartCoroutine(SpawnBomb());
    }

    IEnumerator SpawnBomb()
    {
        

        yield return new WaitForSeconds(2);
        
        for(int i=0;i<bombPoints.Length;i++) { 
            if (Bombused[i])
            {
                GameObject bomb=Instantiate(bombfactory);
                bomb.transform.position = bombPoints[i].transform.position;
                Bombused[i] = false;
            }
        }

        
    }

    public void SetBombUsed(int index)
    {
        Bombused[index] = true;
    }
}
