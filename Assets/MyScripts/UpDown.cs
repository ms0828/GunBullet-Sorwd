using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDown : MonoBehaviour
{
    Vector3 pos; // 현재 위치
    float shortDelta = 1.55f; // 위(아래)로 이동가능한 (x)최대값
    float longDelta = 3.55f;
    float shortSpeed = 2.0f; // 이동속도
    float longSpeed = 1.0f;
    float timer = 0;
    float waitingTime = 5;
    bool isStop = false;

    void Start () 
    {
        pos = transform.position;
    }


    void Update ()
    {
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

        v.y += shortDelta * Mathf.Sin(Time.time * shortSpeed);
        transform.position = v;


        if (pos.y - v.y > 1.5)
        {
            StartCoroutine (Pause());
        }
    }

    void LongMove()
    {
        Vector3 v = pos;

        v.y += longDelta * Mathf.Sin(Time.time * longSpeed);
        transform.position = v;  

        if (pos.y - v.y > 3.5)
        {
            StartCoroutine (Pause());
        }
    }

    private void OnColliderEnter2D(Collision2D other) 
    {
        if (other.gameObject.name.Equals("Ground"))
        {
            isStop = true;
        }
    }

    IEnumerator Pause()
    {
        yield return new WaitForSeconds (1f);
    }
}
