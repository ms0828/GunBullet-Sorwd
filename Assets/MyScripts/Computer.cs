using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour
{
    bool isTriggerEnter = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && isTriggerEnter == true) // G키를 누르면
        {
            Debug.Log("Computer");
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
