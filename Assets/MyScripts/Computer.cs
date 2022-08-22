using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Computer : MonoBehaviour
{
    public bool isTriggerEnter = false;
    public static bool noLimit = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && isTriggerEnter == true) // G키를 누르면
        {
            noLimit = true;
        }

    }

   private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name.Equals("Player")) 
        {
            isTriggerEnter = true;
        }
    }
}
