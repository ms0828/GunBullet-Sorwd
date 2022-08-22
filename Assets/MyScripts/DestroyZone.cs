using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D rb;
    public int damage = 30;
    public float destroySec = 0.1f; // 플레이어가 발판을 밟고 사라지는데까지 걸리는 시간

    void Start()
    {
        rb = player.GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.name.Equals("Player")) // 플레이어가 발판을 밟으면
        {
            collision.gameObject.GetComponent<ITakeDamage>().TakeDamage(this.transform, damage);

            if(transform.position.x - player.transform.position.x > 0)
            {
                rb.AddForce(new Vector2(-2f,3f), ForceMode2D.Impulse);   
            }        
            else      
            {
                rb.AddForce(new Vector2(2f,3f), ForceMode2D.Impulse);       
            }

            Destroy(gameObject, destroySec); // 일정 시간 이후 발판이 삭제됨
        }
    }
}
