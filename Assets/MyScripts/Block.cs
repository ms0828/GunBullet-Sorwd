using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    Rigidbody2D rb;


    float timer = 0;
    float fallSec = 0.3f;
    float destroySec = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

        void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.name.Equals("Player")) // 플레이어가 블럭을 밟으면
        {
            Invoke("FallBlock", fallSec); // 블럭이 떨어지고
            Destroy(gameObject, destroySec);  // 1초 뒤 삭제됨
        }
    }

    void FallBlock()
    {
        rb.isKinematic = false;
    }
}
