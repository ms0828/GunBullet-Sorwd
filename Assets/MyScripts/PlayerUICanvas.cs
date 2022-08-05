using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUICanvas : MonoBehaviour
{

    public GameObject[] hpBar;
    public GameObject[] bulletCount;



    public void SetPlayerHpBar(int currentHp)
    {
        int currentHpBarIndex = (int)(currentHp * 0.1);
 
        for(int i = 0; i < 20; i++)
        {   
            if(i<currentHpBarIndex)
                hpBar[i].SetActive(true);
            else
                hpBar[i].SetActive(false);
        }
    }


    public void SetPlayerBulletCount(int bulletCnt)
    {
        for(int i=0; i < 3; i++)
        {
            if(i < bulletCnt)
                bulletCount[i].SetActive(true);
            else
                bulletCount[i].SetActive(false);
        }
    }
}
