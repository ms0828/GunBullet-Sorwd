using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IInterfaceObject
{    
    public Player player;


    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void Interface()
    {
        player.currentHp += 30;
        if(player.currentHp > player.maxHp)
            player.currentHp = player.maxHp;

        player.playerUi.SetPlayerHpBar(player.currentHp);

        Destroy(gameObject);
    }

   
}
