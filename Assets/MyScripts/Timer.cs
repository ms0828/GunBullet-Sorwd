using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float limitTime = 907;
    int min = 0;
    float sec = 0;
    public Text timer;
    bool value;

    void Update()
    {
        if (Computer.noLimit != true)
        {
            time();
        }
        else
        {
            timer.text = "No Limit";
        }
    }

    void time()
    {       
        limitTime -= Time.deltaTime;

        if (limitTime >= 60f && limitTime != -1) // 남은 시간이 60초 이상일 때
        {
            min = (int)limitTime / 60;
            sec = limitTime % 60;
            timer.text =  min + " : " + (int)sec;
        }
        
        else if (limitTime < 60f && limitTime != -1) // 남은 시간이 60초 미만일 (0 이상) 때
        {
            timer.text = "0 : " + (int)limitTime;
        }
        
        else if (limitTime == 0) // 남은 시간이 0일 때
        {
            timer.text = "0 : 0";
        }
    }
}
