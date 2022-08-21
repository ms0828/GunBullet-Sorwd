using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Computer : MonoBehaviour
{
    public bool isTriggerEnter = false;
    public bool isNoLimit = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && isTriggerEnter == true) // G키를 누르면
        {
            isNoLimit = true;
        }

        if (isNoLimit == true)
        {
            ReturnTimer(true);
        }
        else
        {
            ReturnTimer(false);
        }
    }

   private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name.Equals("Player")) 
        {
            isTriggerEnter = true;
        }
    }

    public bool ReturnTimer(bool value)
    {
        return value;
    }
}
