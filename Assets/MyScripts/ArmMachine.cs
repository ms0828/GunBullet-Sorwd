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
    public float knockbackSpeed = 3;     // 넉백 속도 조절
    public float knockbackTime = 0.2f;       // 넉백 시간 조절
    public float snipingTime = 3f;      // 저격 시간 조절

    public bool isknockback = false;
    public bool isSniping = false;

    new void Start()
    {
        base.Start();   //Enemy의 Start 먼저 실행

        maxHp = 100;
        currentHp = 100;
        shortAttackPower = 15;
        longAttackPower = 20;

        playerLayer = 1 << LayerMask.NameToLayer("Player");


    }

    void Update()
    {
        ShortAttack();
    }

    // ----- 적 근거리 공격 후 넉백 관련 -----
    void Knockback(float dir)
    {
        isknockback = true;
        float time = 0;
        
        while (time < knockbackTime)
        {
            if (transform.rotation.y == 0)      // 플레이어 위치의 반대 방향으로 넉백      
                transform.Translate(Vector2.left * knockbackSpeed * Time.deltaTime * dir);
            else
                transform.Translate(Vector2.left * knockbackSpeed * Time.deltaTime * -1f * dir);
        
            time += Time.deltaTime;
        }

        isknockback = false;
    }

    void Sniping()
    {
        isSniping = true;
        float time = 0;

        while (time < snipingTime)      // 레이저 조준하는 시간
        {
            time += Time.deltaTime;
        }

        isSniping = false;
    }

    public override void ShortAttack()     // 근접 공격
    {
        Collider2D collider1 = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,playerLayer);
        
        if(collider1 != null)    // 플레이어가 있으면 
        {   
            am.SetTrigger("KnifeAttack");

            if(transform.position.x - player.position.x > 0)  // 대상이 오른쪽에 있다면
            {
                Knockback(1);       
            }
            else      // 대상이 왼쪽에 있다면
            {
                Knockback(-1);  
            }
        }
    }

    public override void LongAttack()      // 원거리 공격
    {
        Collider2D collider2 = Physics2D.OverlapBox(SnipingScope.position,SnipingScope.localScale,0,playerLayer);

        if(collider2 != null)
        {
            am.SetTrigger("Sniping");

            Sniping();
        }
    }

        private void OnDrawGizmos()     
    {                               
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(SnipingScope.position,SnipingScope.localScale);
    }
}
