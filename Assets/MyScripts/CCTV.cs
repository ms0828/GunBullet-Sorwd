using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CCTV : MonoBehaviour
{
    public GameObject left;
    public GameObject center;
    public GameObject right;

    public bool isLeft = false;
    public bool isCenter = false;
    public bool isRight = false;
    public bool isNone = false;


    float timer = 0;

    void Update() 
    {
        CameraOnOff();
    }

    void CameraOnOff()
    {
        timer += Time.deltaTime;

        if (timer > 2 && isLeft == false) // 왼쪽이 켜짐
        {
            center.SetActive(false);
            left.SetActive(true);
            isLeft = true;
        }

        if (timer > 4 && isRight == false) // 오른쪽이 켜짐
        {
            left.SetActive(false);
            right.SetActive(true);
            isRight = true;
        }

        if (timer > 6 && isNone == false) // 카메라가 꺼짐
        {
            right.SetActive(false);
            isNone = true;
        }

        if (timer > 8 && isCenter == false) // 중간이 켜짐
        {
            center.SetActive(true);
            isCenter = true;
        }

        if (timer > 10) // 한 바퀴 반복 완료
        {
            timer = 0;

            isCenter = false;
            isLeft = false;
            isRight = false;
            isNone = false;
        }
    }
}
