using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    public Animator am;
    private Vector2 direction;

    public Transform explosionHitBox;

    private Vector2 explosionScale = new Vector2(5,5);

    //------오디오-------
    public AudioSource attackAudio;
    

    public void SetGrenade(int direction)
    {
        attackAudio = transform.Find("AttackAudio").GetComponent<AudioSource>();

        if(direction > 0)    //오른쪽 방향이면 이미지 좌우 반전 적용
        {
            sr.flipX = true;
        }

        rb.AddForce(new Vector2(5f * direction, 3f), ForceMode2D.Impulse);       //(impulse => 순간적으로 힘을 준다)

        Invoke("Explosion", 3f);
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            Explosion();
        }
    }

    public void Explosion()
    {
        SoundManager.instance.EnemySfxSound(attackAudio,"HeadMachineGrenade");

        am.SetTrigger("Explosion");
        transform.localScale = explosionScale;
        Destroy(gameObject, 0.5f);

        Collider2D collider = Physics2D.OverlapBox(explosionHitBox.position, explosionHitBox.localScale,0, 1 << LayerMask.NameToLayer("Player"));

        if(collider != null)
        {
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, 20);
        }
    }

    

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(explosionHitBox.position, explosionHitBox.localScale);
    }
}
