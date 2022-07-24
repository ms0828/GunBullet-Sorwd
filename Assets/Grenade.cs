using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    public Animator am;
    private Vector2 direction;


    public void SetGrenade(int direction)
    {
        if(direction > 0)    //오른쪽 방향이면 이미지 좌우 반전 적용
        {
            sr.flipX = true;
        }

        rb.AddForce(new Vector2(5f * direction, 3f), ForceMode2D.Impulse);       //(impulse => 순간적으로 힘을 준다)

        Invoke("Explosion", 3f);
    }

    /*void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            Explosion();
        }
    }*/

    public void Explosion()
    {
        am.SetTrigger("Explosion");
        Destroy(gameObject, 1f);
    }
}
