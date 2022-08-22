using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMachine : Enemy, ITakeDamage
{

    //------스탯 관련------
    public int grabAttackPower = 50;
    public float grenadeDistance = 4f;

    //------상태 관련 bool-------
    public bool isGrab = false;
    public bool isGrabCoolTime = false;
    public bool isThrowCoolTime = false;


    //------필요 Object or Transform------
    public Transform throwPos;
    public Transform throwScope;
    public GameObject grenadePrefeb;
    public Transform grabScope;    //잡기 범위
    public Transform muzzlePos;


    //------쿨타임 코루틴 캐싱------
    public WaitForSeconds grabCoolTime = new WaitForSeconds(5.0f);
    public WaitForSeconds throwCoolTime = new WaitForSeconds(8.0f);


    //------오디오-------
    public AudioSource attackAudio;
    public AudioSource behaviorAudio;


    new void Awake()
    {
        base.Awake();
        
        attackAudio = transform.Find("AttackAudio").GetComponent<AudioSource>();
        behaviorAudio = transform.Find("BehaviorAudio").GetComponent<AudioSource>();
    }

    void Start()
    {
        maxHp = 200;
        currentHp = 200;
        speed = 1f;
        trackingDistance = 8f;

        mask = LayerMask.GetMask("Player") | LayerMask.GetMask("Block");

        Invoke("SetMoveDirection", 0.5f);
    }


    void FixedUpdate()
    {
        if(isDead)
            return;

        Move();
        ObservePlayer();
    }




    //-----------움직임 방향 설정 관련---------------
    void SetMoveDirection()     //몬스터 움직임 방향을 3초마다 설정
    {
        nextMove = Random.Range(-1,2);      //-1~1까지 

        int time = Random.Range(1,5);
        Invoke("SetMoveDirection", time);       //다음 방향 설정
    }

    IEnumerator GoBack()    //반대로 가는 함수
    {
        yield return new WaitForSeconds(1.0f);
        nextMove *= -1;
    }



    /*
    //플레이어 추적 이동을 트리거로 구현
    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            CancelInvoke("SetMoveDirection");       //랜덤 방향 설정 취소하고
        }
    }

    void OnTriggerStay2D(Collider2D other)      //캐릭터 위치 추적
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            if(other.transform.position.x - transform.position.x > 0)   //캐릭터가 오른쪽에 있다면
            {
                nextMove = 1;
            }
            else
            {
                nextMove = -1;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag.Equals("Player"))   //플레이어가 인식 범위에서 벗어나면
        {
            if(am.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                //StartCoroutine("GoBack");   //1초 후 반대 방향으로 감
            }
            Invoke("SetMoveDirection",3f);      //3초후 랜덤 방향 다시 설정
        }
    }*/

    void OnTriggerStay2D(Collider2D other)      //캐릭터 위치 추적
    {

        if(other.gameObject.tag.Equals("Player"))
        {
            RaycastHit2D rayHit;

            if(other.transform.position.x - transform.position.x > 0)   //캐릭터가 오른쪽에 있다면
            {
                rayHit = Physics2D.Raycast(muzzlePos.position, new Vector2(1, 0), trackingDistance, mask);
            }
            else
            {
                rayHit = Physics2D.Raycast(muzzlePos.position, new Vector2(-1, 0), trackingDistance, mask);
            }


            if(rayHit.collider != null && rayHit.collider.CompareTag(playerTag))   //플레이어가 감지되면
            {
                CancelInvoke("SetMoveDirection");       //랜덤 방향 설정 취소하고

                if(other.transform.position.x - transform.position.x > 0)   //캐릭터가 오른쪽에 있다면
                {
                    nextMove = 1;
                }
                else
                {
                    nextMove = -1;
                }
            }
            else        //플레이어가 감지 되지 않으면 (벽에 막히거나, 플레이어가 인식 범위를 벗어나면)
            {   
                if(IsInvoking("SetMoveDirection") == false)
                {
                    Invoke("SetMoveDirection",2f);      //3초후 랜덤 방향 다시 설정
                }
            }
        }

    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag.Equals("Player"))   //플레이어가 인식 범위에서 벗어나면
        {
            if(IsInvoking("SetMoveDirection") == false)
            {
                Invoke("SetMoveDirection",3f);      //3초후 랜덤 방향 다시 설정
            }
        }
    }
    //-----------------------------------------------


    


    void Move()     //몬스터 움직임 함수
    {   
        //몬스터의 앞 방향 벡터 (낭떨어지 체크)
        Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.3f, rb.position.y);

        //몬스터의 앞에서 아래로 레이캐스트를 쏴서 아래가 낭떨어지인지 확인함
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 2f ,LayerMask.GetMask("Block"));
        if(rayHit.collider == null)  //낭떨어지이면
        {
            nextMove *= -1;     //다음 움직임 방향을 반대 방향으로
            CancelInvoke("SetMoveDirection");
            Invoke("SetMoveDirection",3);       //3초후, 움직임 방향 다시 설정 
        }

        
        if(nextMove == 0)       //정지
        {
            am.SetBool("Run",false);
        }
        else if(nextMove == -1)   //왼쪽으로 갈 때
        {
            am.SetBool("Run",true);
            transform.localScale = leftDirection;  //-----이미지 방향 처리-----
        }
        else     //오른쪽으로 갈 때
        {
            am.SetBool("Run",true);
            transform.localScale = rightDirection;  //-----이미지 방향 처리-----
        }


        //움직이는 에니메이션 실행 중에 이동방향으로 이동처리
        if(am.GetCurrentAnimatorStateInfo(0).IsName("Run")) //달리는 에니메이션 진행중이면
        {
            transform.Translate(new Vector2(nextMove * Time.deltaTime * speed ,0));
        }
        
    }




    //공격 루트 : ObervePlayer -> Grab -> GrabAttack (홀딩을 풀지 못하면)
    void ObservePlayer()        //플레이어가 공격 범위 안에 들어오면 잡기 에니메이션 실행
    {
        //-----근접 공격(잡기)------
        //플레이어의 레이어(인덱스)를 가져온 후, 히트박스 내 해당 레이어를 가진 충돌체(몬스터)들을 배열로 가져옴
        Collider2D collider1 = Physics2D.OverlapBox(grabScope.position,grabScope.localScale,0,playerLayer);
        
        if(collider1 != null && isGrabCoolTime == false)    //플레이어가 있으면
        {   
            isGrab = true;
            am.SetBool("Grab", true);
            StartCoroutine(GrabCoolTime());     //잡기 쿨타임
            return;
        }

        
        //-----원거리 공격(수류탄 투척)------
        /*
        Collider2D collider2 = Physics2D.OverlapBox(throwScope.position,throwScope.localScale,0,playerLayer);
        
        if(collider2 != null && isThrowCoolTime == false && isGrab == false)    //플레이어가 있으면
        {   
            am.SetTrigger("Throw");
            StartCoroutine(ThrowCoolTime());    //투척 쿨타임
        }
        */

        RaycastHit2D rayHit;
        if(transform.localScale.x > 0)  //오른쪽 보고 있을 때
            rayHit = Physics2D.Raycast(muzzlePos.position, new Vector2(1, 0), grenadeDistance, mask);
        else
            rayHit = Physics2D.Raycast(muzzlePos.position, new Vector2(-1, 0), grenadeDistance, mask);
        
        if(rayHit.collider != null && rayHit.collider.CompareTag(playerTag) && isThrowCoolTime == false && isGrab == false)     //현재 바라보는 방향 일직선에서 플레이어가 감지되면
        {
            am.SetTrigger("Throw");
            StartCoroutine(ThrowCoolTime());    //투척 쿨타임
        }
    }


    public void Grab()      //잡기 에니메이션에서 실행
    {
        
        Collider2D collider = Physics2D.OverlapBox(grabScope.position,grabScope.localScale,0,playerLayer);
        
        if(collider != null)    //플레이어가 있으면
        {   
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
            Collider2D collider = Physics2D.OverlapBox(grabScope.position,grabScope.localScale,0,playerLayer);
            
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


    public void ShortAttack()      //근거리 공격       //잡기 공격 에니메이션에서 실행됨
    {
        SoundManager.instance.EnemySfxSound(attackAudio,"HeadMachineShoot");

        Collider2D collider = Physics2D.OverlapBox(grabScope.position,grabScope.localScale,0,playerLayer);
        
        if(collider != null)    //플레이어가 있으면
        {   
            //ITakeDamage 인터페이스를 가진 대상으로 인터페이스 함수(TakeDamage) 실행
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, grabAttackPower);     //대상의 TakeDamage 함수 실행
        }
    }


    //------------쿨타임 관련-------------
    IEnumerator GrabCoolTime()
    {
        isGrabCoolTime = true;

        yield return grabCoolTime;

        isGrabCoolTime = false;
    }

    IEnumerator ThrowCoolTime()
    {
        isThrowCoolTime = true;

        yield return throwCoolTime;

        isThrowCoolTime = false;
    }




    public void TakeDamage(Transform attacker, int damage)
    {

        if(Random.Range(0,2) == 0)
            SoundManager.instance.EnemySfxSound(behaviorAudio,"HeadMachineHit1");
        else
            SoundManager.instance.EnemySfxSound(behaviorAudio,"HeadMachineHit2");


        //잡는 도중에 맞으면 잡기 상태 풀고, 플레이어도 홀딩 상태 해제
        if(isGrab == true)
        {
            isGrab = false;
            am.SetBool("Grab", false);

            player.isHolding = false;
            player.am.SetBool("Holding",false);
        }
        //-------
         
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
                SoundManager.instance.EnemySfxSound(behaviorAudio,"HeadMachineDie");

                currentHp = 0;
                isDead = true;
                //am.SetTrigger("Dead");
    
                Destroy(gameObject, 2f);        //2초 후 사라짐
            }
        }
    }

    /*
    //-------수류탄 투척 범위 나타내기-------(인게임에서는 안보임) (크기 조절하기 위해 만든 함수)
    private void OnDrawGizmos()     
    {                               
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(throwScope.position,throwScope.localScale);
    }
    */



    //-------히트박스 나타내기-------(인게임에서는 안보임) (히트박스 크기 조절하기 위해 만든 함수)
    private void OnDrawGizmos()     
    {                               
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(grabScope.position,grabScope.localScale);
    }


}
