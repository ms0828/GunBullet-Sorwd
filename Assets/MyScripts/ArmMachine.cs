using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmMachine : Enemy, ITakeDamage
{
    public Transform player;
    public Transform aimStartPoint;
    public GameObject aimLaser;

    int playerLayer;
    float direct;
    public float knockbackTime = 1.0f;       // 넉백 시간 조절


    public bool isknockback = false;
    public bool isKnifeCoolTime = false;
    public bool isSnipingCoolTime = false;
    
    public bool isAiming = false;

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

        //ShortAttack();
        ObservePlayer();
    }



    // ----- 적 근거리 공격 후 넉백 관련 -----
    void Knockback()
    {
        isknockback = true;
        
        if(transform.position.x - player.position.x > 0)    // 플레이어 위치의 반대 방향으로 넉백                    
            rb.AddForce(new Vector2(3f,2f), ForceMode2D.Impulse);
        else
            rb.AddForce(new Vector2(-3f,2f), ForceMode2D.Impulse);  
        

        isknockback = false;
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

        yield return new WaitForSeconds(5.0f);

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
         
        }
    }

    public override void LongAttack()      // 원거리 공격 (저격 후, 총 발사)
    {
        
    }


    void ObservePlayer()       //플레이어 감지
    {
        if(isAiming == true)    //현재 조준 중이면 함수 종료
            return;


        RaycastHit2D rayHit = Physics2D.Raycast(aimStartPoint.position, new Vector2(nextMove, 0), 5f, LayerMask.GetMask("Player"));

        Debug.DrawRay(aimStartPoint.position, new Vector2(nextMove * 5f,0), Color.yellow);


        if(rayHit.collider == null)
            return;
        else    //일직선 방향으로 레이캐스트를 쏴서 플레이어가 감지되면
        {
            if(isSnipingCoolTime == false)
            {
                StartCoroutine(AimingPlayer());
            }
        }

        
    }

    IEnumerator AimingPlayer()  //플레이어 조준
    {
        StartCoroutine(SnipingCoolTime());

        isAiming = true;
        am.SetBool("Aim", true);
        aimLaser.SetActive(true);

        for(int i=0; i<10; i++)
        {
            //플레이어가 공격 범위 안에 있는지 0.3초 단위로 반복 검사
            Vector2 currentVec = transform.position;
            RaycastHit2D rayHit = Physics2D.Raycast(currentVec,new Vector2(nextMove,0),5f,LayerMask.GetMask("Player"));

            if(rayHit.collider == null || isAiming == false)        //피격 시 isAiming => false  -> 조준 해제
            {
                aimLaser.SetActive(false);
                isAiming = false;
                am.SetBool("Aim", false);
                yield break;
            }

            yield return new WaitForSeconds(0.3f);
        }

        if(isAiming == true)  //발사
        {
            aimLaser.SetActive(false);
            isAiming = false;
            am.SetBool("Aim", false);
            am.SetTrigger("Shoot");
        }
      
    }


    void Shoot()        //조준 후, 발사 Shoot 에니메이션에서 발동 (에니메이션 이벤트 함수)
    {
        
    }



    public void TakeDamage(Transform attacker, int damage)
    {  
        //조준 도중에 맞으면 조준 상태 해제
        if(isAiming == true)
            isAiming = false;
        

        if(currentHp - damage > 0)      //히트
        {
            currentHp = currentHp - damage;
            am.SetTrigger("Hit");

            //넉백
            if(transform.position.x - attacker.position.x > 0)  //대상이 왼쪽에서 공격했다면
            {
                rb.AddForce(new Vector2(3f,0f), ForceMode2D.Impulse);       //(impulse => 순간적으로 힘을 준다)
            }
            else      //대상이 오른쪽에서 공격했다면
            {
                rb.AddForce(new Vector2(-3f,0f), ForceMode2D.Impulse);       //(impulse => 순간적으로 힘을 준다)
            }

        }
        else       //사망
        {
            if(!isDead)     //이미 죽은 상태에서 피격 방지
            {
                currentHp = 0;
                isDead = true;
                //am.SetTrigger("Dead");
    
                Destroy(gameObject, 2f);        //2초 후 사라짐
            }
        }
    }

 
}