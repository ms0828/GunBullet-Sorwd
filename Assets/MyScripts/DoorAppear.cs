using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAppear : MonoBehaviour
{
    public GameObject Door;

    private void OnTriggerEnter2D(Collider2D other) // 플레이어가 범위 내에 들어오면
    {
        if(other.gameObject.name.Equals("Player")) 
        {
            Door.SetActive(true); // 문을 나타냄
        }
    }
}
