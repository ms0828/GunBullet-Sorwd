using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUI : MonoBehaviour
{
    public GameObject[] hpBar;



    public void SetBossHpBar(int currentHp)
    {
        
        int currentHpBarIndex = (int)(currentHp / 40);

        for(int i = 0; i < 20; i++)
        {   
            if(i<currentHpBarIndex)
                hpBar[i].SetActive(true);
            else
                hpBar[i].SetActive(false);
        }
    }
}
