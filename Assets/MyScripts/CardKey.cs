using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardKey : MonoBehaviour
{
    public static bool isCardKey = false;
    public bool isCardExist = false;
    void Update()
    {
        if (ArmMachine.isCardExist == true)
        {
            gameObject.SetActive(true);
        }

        if (isCardExist == true)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                gameObject.SetActive(false);
                isCardKey = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.name.Equals("Player")) 
        {
            isCardExist = true;  
        }
    }
}
