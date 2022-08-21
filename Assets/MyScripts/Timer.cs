using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : Computer
{
    public float limitTime = 900;
    int min = 0;
    float sec = 0;
    public Text timer;
    public Computer computer;
    bool value;

    void Start()
    {
        computer =  GameObject.Find("Computer").GetComponent<Computer>();
    }

    void Update()
    {
        time();
    }

    void time()
    {
        isNoLimit = computer.ReturnTimer(value);

        if (isNoLimit == false)
        {
            limitTime -= Time.deltaTime;
        }
        
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

        else if (isNoLimit == true) // 컴퓨터와 상호작용 이후 시간 제한이 사라짐
        {
            timer.text = "No Limit";
        }
    }
}
