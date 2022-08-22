using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDown : MonoBehaviour
{   
    public Rigidbody2D rbPlayer;
    public Rigidbody2D rbEnemy;
    public GameObject player;
    public GameObject enemy;
    Vector3 pos; // 현재 위치
    Vector3 playerDistance;
    Vector3 enemyDistance;

    float shortDelta = 1.55f; // 위(아래)로 이동 가능한 최대값
    float longDelta = 3.55f;
    float shortSpeed = 2.0f; // 이동속도
    float longSpeed = 1.0f;
    
    public bool dir = false;

    void Start () 
    {
        pos = transform.position;
        player = GameObject.FindWithTag("Player");
        rbPlayer = player.GetComponent<Rigidbody2D>();
    }


    void Update ()
    {
        playerDistance = gameObject.transform.position - player.transform.position;
        if (playerDistance.y < 0)
        {
            playerDistance.y = 10;
        }

        if (this.gameObject.tag.Equals("ShortJumpZone"))
        {
            ShortMove();
        }
        if (this.gameObject.tag.Equals("LongJumpZone"))
        {
            LongMove();
        }
    }

    void ShortMove()
    {
        Vector3 v = pos;

        if (dir == true && playerDistance.y < 1.0)
        {
            if (playerDistance.x > 0)
            {
                rbPlayer.AddForce(new Vector2(-2.5f,0), ForceMode2D.Impulse);
            }
            else
            {
                rbPlayer.AddForce(new Vector2(2.5f,0), ForceMode2D.Impulse);
            }

            dir = false;
        }
        else
        {
            v.y += shortDelta * Mathf.Sin(Time.time * shortSpeed);
            transform.position = v;
        }
    }

    void LongMove()
    {
        Vector3 v = pos;

        if (dir == true && playerDistance.y < 1.0)
        {
            if (playerDistance.x > 0)
            {
                rbPlayer.AddForce(new Vector2(-2.5f,0), ForceMode2D.Impulse);
            }
            else
            {
                rbPlayer.AddForce(new Vector2(2.5f,0), ForceMode2D.Impulse);
            }

            dir = false;
        }
        else
        {
            v.y += longDelta * Mathf.Sin(Time.time * longSpeed);
            transform.position = v;  
        }

    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            rbEnemy = other.collider.GetComponent<Rigidbody2D>();

            if (enemyDistance.x > 0)
            {
                rbEnemy.AddForce(new Vector2(3f,0), ForceMode2D.Impulse);
            }
            else
            {
                rbEnemy.AddForce(new Vector2(3f,0), ForceMode2D.Impulse);
            } 
        }
    }
}

