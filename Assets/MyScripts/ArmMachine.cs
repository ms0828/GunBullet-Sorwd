using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmMachine : Enemy
{
    public Transform player;
    public Transform SnipingScope;

    int playerLayer;
    float direct;
    public float knockbackTime = 1.0f;       // 넉백 시간 조절
    public float snipingTime = 4.0f;      // 저격 시간 조절

    public bool isknockback = false;
    public bool isSniping = false;
    public bool isKnifeCoolTime = false;
    public bool isSnipingCoolTime = false;    

    new void Start()
    {
        base.Start();   //Enemy의 Start 먼저 실행

        maxHp = 100;
        currentHp = 100;
        speed = 1f;
        shortAttackPower = 15;
        longAttackPower = 10;

        playerLayer = 1 << LayerMask.NameToLayer("Player");
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();

        // ShortAttack();
        LongAttack(); 
    }



    // ----- 적 근거리 공격 후 넉백 관련 -----
    void Knockback()
    {
        isknockback = true;
        float time = 0;

        
        Debug.Log(transform.position.x - player.position.x);

        while (time < knockbackTime)
        {
            if(transform.position.x - player.position.x > 0)    // 플레이어 위치의 반대 방향으로 넉백                    
                rb.AddForce(new Vector2(0.4f,0f), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(-0.4f,0f), ForceMode2D.Impulse);  
        
            time += Time.deltaTime;
        }

        isknockback = false;
    }
    // ----- 적 원거리 공격 관련 -----
    void Sniping()
    {
        isSniping = true;
        speed = 0;      // 총을 조준하는 동안 적은 움직이지 않음
        float time = 0;

        am.SetTrigger("Sniping");

        while (time < snipingTime)      // 총을 조준하는 시간 (레이저가 발사됨)
        {   
            time += Time.deltaTime;
        }

        speed = 1f;
        isSniping = false;
    }
    
    // ----- 쿨타임 관련 -----
        IEnumerator KnifeCoolTime()
    {
        isKnifeCoolTime = true;

        yield return new WaitForSeconds(5.0f);

        isKnifeCoolTime = false;
    }
        IEnumerator SnipingCoolTime()
    {
        isSnipingCoolTime = true;

        yield return new WaitForSeconds(8.0f);

        isSnipingCoolTime = false;
    }

    public override void ShortAttack()     // 근접 공격
    {
        Collider2D collider1 = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,playerLayer);
        
        if(collider1 != null && isKnifeCoolTime == false)    // 히트박스 범위 내에 플레이어가 있으면 
        {   
            StartCoroutine(KnifeCoolTime());
            am.SetTrigger("KnifeAttack");
            collider1.GetComponent<ITakeDamage>().TakeDamage(player.transform, shortAttackPower);
            Knockback();            
        }
    }

    public override void LongAttack()      // 원거리 공격
    {
        Collider2D collider2 = Physics2D.OverlapBox(SnipingScope.position,SnipingScope.localScale,0,playerLayer);

        if(collider2 != null && isSnipingCoolTime == false)
        {
            StartCoroutine(SnipingCoolTime());
            am.SetTrigger("Sniping");
            // collider2.GetComponent<ITakeDamage>().TakeDamage(player.transform, longAttackPower);
            Sniping();
        }
    }

        // ----- 원거리 저격 범위 -----
        private void OnDrawGizmos()     
    {                               
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(SnipingScope.position,SnipingScope.localScale);
    }
}