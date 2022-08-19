using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Elevator : MonoBehaviour
{
    Vector3 pos; // 현재 위치
    float shortDelta = 1.55f; // 좌(우)로 이동가능한 (x)최대값
    float longDelta = 3.5f;
    float speed = 2.0f; // 이동속도
    float timer = 0;
    float waitingTime = 5;
    bool isStop = false;

    void Start () 
    {
        pos = transform.position;
    }


    void Update ()
    {
        if (this.gameObject.tag.Equals("ShortJumpZone"))
        {
            ShortMove();
        }
        if (this.gameObject.tag.Equals("LongJumpZone"))
        {
            LongMove();
        }
    }

    void ShortMove()
    {
        Vector3 v = pos;
        double po = Math.Truncate(v.y * 10) / 10;

        v.y += shortDelta * Mathf.Sin(Time.time * speed);
        transform.position = v;
    }

    void LongMove()
    {
        Vector3 v = pos;
        double po = Math.Truncate(v.y * 10) / 10;

        v.y += longDelta * Mathf.Sin(Time.time * speed);
        transform.position = v;  
    }
}
