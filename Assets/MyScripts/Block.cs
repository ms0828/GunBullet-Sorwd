using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public GameObject player;
    public GameObject block;
    Rigidbody2D rb;
    Vector2 pos;
    float fallSec = 0.3f;
    float destroySec = 1f;
    bool isFall = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        pos = gameObject.transform.position;
    }

    void FixedUpdate()
    {
        if (isFall == true)
        {
            StartCoroutine("Respawn");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("Player")) // 플레이어가 블럭을 밟으면
        {
           if (player.transform.position.y > gameObject.transform.position.y)
           {
                Invoke("FallBlock", fallSec);
                isFall = true;
           }
        }
    }

    void FallBlock()
    {
        rb.isKinematic = false;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds (2f); // 2초 뒤 블럭 재생성
        gameObject.SetActive(false);
        gameObject.transform.position = pos;
        rb.isKinematic = true;
        gameObject.SetActive(true);

        isFall = false;
    }
}
