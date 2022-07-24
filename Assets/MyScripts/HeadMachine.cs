using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMachine : Enemy
{
    private Player player;      //최초 잡기 시전 때, collider를 통해 캐싱해서 사용  (연산 줄이기 위해)

    int playerLayer;

    public bool isGrab = false;
    public bool isGrabCoolTime = false;
    public bool isThrowCoolTime = false;

    public Transform throwPos;

    public Transform throwScope;

    public GameObject grenadePrefeb;

    new void Start()
    {
        base.Start();
        
        maxHp = 120;
        currentHp = 120;
        speed = 1f;
        shortAttackPower = 20;
        longAttackPower = 15;
        
        playerLayer = 1 << LayerMask.NameToLayer("Player");
    }


    

    void Update()
    {
        
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();

        ObservePlayer();
    }


    //공격 루트 : ObervePlayer -> Grab -> GrabAttack (홀딩을 풀지 못하면)
    void ObservePlayer()        //플레이어가 공격 범위 안에 들어오면 잡기 에니메이션 실행
    {
        //-----근접 공격(잡기)------
        //플레이어의 레이어(인덱스)를 가져온 후, 히트박스 내 해당 레이어를 가진 충돌체(몬스터)들을 배열로 가져옴
        Collider2D collider1 = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,playerLayer);
        
        if(collider1 != null && isGrabCoolTime == false)    //플레이어가 있으면
        {   
            isGrab = true;
            am.SetBool("Grab", true);
            StartCoroutine(GrabCoolTime());     //잡기 쿨타임
        }

        
        //-----원거리 공격(수류탄 투척)------
        Collider2D collider2 = Physics2D.OverlapBox(throwScope.position,throwScope.localScale,0,playerLayer);
        
        if(collider2 != null && isThrowCoolTime == false && isGrab == false)    //플레이어가 있으면
        {   
            am.SetTrigger("Throw");
            StartCoroutine(ThrowCoolTime());    //투척 쿨타임
        }
        
    }


    public void Grab()      //잡기 에니메이션에서 실행
    {
        
        Collider2D collider = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,playerLayer);
        
        if(collider != null)    //플레이어가 있으면
        {   
            if(player == null)  //최초 잡기 시전 때 플레이어 캐싱
            {
                player = collider.GetComponent<Player>();   
            }

            if(player.isHolding == false)
            {
                StartCoroutine(player.Holding(this));       //플레이어 홀딩 함수 발동
                StartCoroutine(GrabAttack());       //잡기 공격 대기
            }
            else        //플레이어가 이미 다른 적에게 잡혀있다면 잡기 취소
            {
                isGrab = false;
                am.SetBool("Grab", false);
            }
            
        }
        else        //잡기 에니메이션 동안 플레이어가 도망갔다면 잡기 취소
        {
            isGrab = false;
            am.SetBool("Grab", false);
        }
    }

    IEnumerator GrabAttack()    
    {

        for(int i=0; i<3; i++)
        {
            Collider2D collider = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,playerLayer);
            
            if(collider == null || player.isHolding == false)    //범위 내 플레이어가 없으면 잡기 종료
            {
                isGrab = false;
                am.SetBool("Grab", false);
                yield break;
            }
            yield return new WaitForSeconds(1.0f);
        }


        //플레이어가 홀딩을 풀지 못하면,
        if(player.isHolding == true)
        {
            am.SetBool("Grab", false);
            am.SetTrigger("GrabAttack");   
        }
        else    //플레이어가 홀딩을 풀었으면
        {
            am.SetBool("Grab", false);
        }

        isGrab = false;
    }

    
    public void ThrowGrenade()
    {   

        Grenade grenade = Instantiate(grenadePrefeb, throwPos.position, throwPos.rotation).GetComponent<Grenade>();
        if(this.transform.localScale.x > 0)    //오른쪽 보고 있으면
        {
            grenade.SetGrenade(nextMove);
        }
        else    //왼쪽 보고 있으면
        {
            grenade.SetGrenade(nextMove);
        }


    }


    //------------쿨타임 관련-------------
    IEnumerator GrabCoolTime()
    {
        isGrabCoolTime = true;

        yield return new WaitForSeconds(5.0f);

        isGrabCoolTime = false;
    }

    IEnumerator ThrowCoolTime()
    {
        isThrowCoolTime = true;

        yield return new WaitForSeconds(7.0f);

        isThrowCoolTime = false;
    }



    public override void ShortAttack()      //근거리 공격       //잡기 공격 에니메이션에서 실행됨
    {
        Collider2D collider = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,playerLayer);
        
        if(collider != null)    //플레이어가 있으면
        {   
            //ITakeDamage 인터페이스를 가진 대상으로 인터페이스 함수(TakeDamage) 실행
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, shortAttackPower);     //대상의 TakeDamage 함수 실행
        }
    }

    public override void LongAttack()      //원거리 공격
    {

    }


    //-------수류탄 투척 범위 나타내기-------(인게임에서는 안보임) (크기 조절하기 위해 만든 함수)
    private void OnDrawGizmos()     
    {                               
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(throwScope.position,throwScope.localScale);
    }
}
