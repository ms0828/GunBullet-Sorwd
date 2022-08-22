using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public SpriteRenderer sr;
    private Vector2 direction;
    private float speed = 13.0f;


    public void SetBullet(Vector2 _direction)
    {
        if(_direction.x < 0)    //왼쪽 방향이면 이미지 좌우 반전 적용
        {
            sr.flipX = true;
        }
        direction = _direction;
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.Translate(direction * Time.deltaTime * speed);
    }


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag.Equals("Ground"))
        {
            Destroy(gameObject);
        }

        if(other.gameObject.tag.Equals("Enemy"))
        {
            other.gameObject.GetComponent<ITakeDamage>().TakeDamage(this.transform, 30);
            Destroy(gameObject);
        
        }
    }
}
