using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    public float destroySec = 1f; // 플레이어가 발판을 밟고 사라지는데까지 걸리는 시간

    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.name.Equals("Player")) // 플레이어가 발판을 밟으면
        {
            Destroy(gameObject, destroySec); // 일정 시간 이후 발판이 삭제됨
        }
    }
}
