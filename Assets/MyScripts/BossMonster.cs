using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : Enemy, ITakeDamage
{
    //-------필요한 기본 할당 변수------
    public PlayerUICanvas playerUi;

    public GameObject table;
    

    //----움직임 제한----
    float minX, maxX, minY, maxY;

    BoxCollider2D mapBoundary;


    //--------스탯 관련 변수-------
    public float trackingDistance;
    public float aimDistance;
    public float crushDistance;

    public int grabAttackPower = 50;
    public int crushAttackPower = 50;

    //------필요 Object or Transform------
    public Transform muzzlePos;
    public GameObject aimLaser;
    public Transform grabScope;    //잡기 범위
    public Transform crushHitBox;    //돌진 공격 범위
    public GameObject bulletPrefeb;


    //---------상태 관련 bool---------
    public bool isTracking = false;     //플레이어 추적 상태
    public bool isAttackCoolTime = false;   //모든 공격 쿨타임
    public bool isGrab = false;
    public bool isGrabCoolTime = false;
    public bool isAiming = false;
    public bool isSnipingCoolTime = false;
    public bool isCharging = false;
    public bool isCrushing = false;
    public bool isCrushCoolTime = false;

    //------오디오-------
    public AudioSource attackAudio;
    public AudioSource behaviorAudio;


    //-----플레이어 캐싱 관련------
    private string playerTag = "Player";
    LayerMask mask;


    //------쿨타임 코루틴 캐싱------
    public WaitForSeconds attackCoolTime = new WaitForSeconds(5.0f);
    public WaitForSeconds grabCoolTime = new WaitForSeconds(3.0f);
    public WaitForSeconds shootCoolTime = new WaitForSeconds(7.0f);
    public WaitForSeconds crushCoolTime = new WaitForSeconds(7.0f);

    public WaitForSeconds chargingWaitTime = new WaitForSeconds(0.3f);
    public WaitForSeconds crushMotionTime = new WaitForSeconds(0.07f);

    new void Awake()
    {
        base.Awake();
        

        if(playerUi == null)
        {
            playerUi = GameObject.Find("PlayerUICanvas").GetComponent<PlayerUICanvas>();
        }
        
        if(attackAudio == null)
        {
            attackAudio = transform.Find("AttackAudio").GetComponent<AudioSource>();
        }

        if(behaviorAudio == null)
        {
            behaviorAudio = transform.Find("BehaviorAudio").GetComponent<AudioSource>();
        }

        if(mapBoundary == null)
        {
            mapBoundary = GameObject.Find("MapBoundary").GetComponent<BoxCollider2D>();
        }

        if(table == null)
        {
            table = GameObject.Find("Stage3_Table");
        }
    }


    void Start()
    {
        SetLimits();    //움직임 영역 제한 설정


        maxHp = 600;
        currentHp = 10;
        speed = 1.3f;
        trackingDistance = 8f;
        aimDistance = 6f;
        crushDistance = 6f;

        mask = LayerMask.GetMask("Player") | LayerMask.GetMask("Block");

        Invoke("SetMoveDirection", 0.5f);

        Destroy(table.GetComponent<Table>());
        table.GetComponent<BoxCollider2D>().enabled = false;
    }


    void FixedUpdate()
    {
        if(isDead)
            return;

        if(playerUi.isPlayTimeline == true)
            return;
        
        Move();
        ObservePlayer();

        //TestDrawRay();        //사거리 테스트 용
    }


    void LateUpdate()   //움직임 영역 제한
    {
        float xClamp = Mathf.Clamp(transform.position.x, minX, maxX);
        float yClamp = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(xClamp, yClamp, transform.position.z);
    }



    //플레이어 추적 이동을 트리거로 구현
  
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

        if(isAiming == true)        //조준 중에는 방향 전환 x
        {
            CancelInvoke("SetMoveDirection");
            return;
        }

        if(isCharging == true || isCrushing == true )
        {
            CancelInvoke("SetMoveDirection");
            return;
        }

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
            transform.localScale = rightDirection;  //-----이미지 방향 처리-----
        }
        else     //오른쪽으로 갈 때
        {
            am.SetBool("Run",true);
            transform.localScale = leftDirection;  //-----이미지 방향 처리-----
        }


        //움직이는 에니메이션 실행 중에 이동방향으로 이동처리
        if(am.GetCurrentAnimatorStateInfo(0).IsName("Run")) //달리는 에니메이션 진행중이면
        {
            transform.Translate(new Vector2(nextMove * Time.deltaTime * speed ,0));
        }
        
    }




    //공격 루트 : ObervePlayer -> Grab or Crush 
    void ObservePlayer()        //플레이어가 공격 범위 안에 들어오면 잡기 에니메이션 실행
    {
        if(isGrab == true)
            return;

        if(isAiming == true)    //현재 조준 중이면 함수 종료
            return;

        if(isCharging == true || isCrushing == true)
            return;


        //-----근접 공격(잡기)------
        //플레이어의 레이어(인덱스)를 가져온 후, 히트박스 내 해당 레이어를 가진 충돌체(몬스터)들을 배열로 가져옴
        Collider2D collider1 = Physics2D.OverlapBox(grabScope.position,grabScope.localScale,0,playerLayer);
        
        if(collider1 != null && isGrabCoolTime == false && isAttackCoolTime == false)    //플레이어가 있으면
        {   
            isGrab = true;
            am.SetBool("Grab", true);
            StartCoroutine(GrabCoolTime());     //잡기 쿨타임
            
            return;
        }






        //-------돌진 공격-------
        RaycastHit2D rayHit1;
        if(transform.localScale.x > 0)  //왼쪽 보고 있을 때
            rayHit1 = Physics2D.Raycast(muzzlePos.position, new Vector2(-1, 0), crushDistance, mask);
        else
            rayHit1 = Physics2D.Raycast(muzzlePos.position, new Vector2(1, 0), crushDistance, mask);

        if(rayHit1.collider != null && rayHit1.collider.CompareTag(playerTag) && isCrushCoolTime == false && isAttackCoolTime == false)     //현재 바라보는 방향 일직선에서 플레이어가 감지되면
        {
            isCharging = true;
            am.SetBool("Charge", true);        //돌진 대기 애니메이션

            StartCoroutine(Charging());
            StartCoroutine(CrushCoolTime());
            StartCoroutine(AttackCoolTime());       //공격 쿨타임

            return;
        }







        //-----적이 멀리있으면 원거리 저격 조준------
        RaycastHit2D rayHit2;
        if(transform.localScale.x > 0)  //왼쪽 보고 있을 때
            rayHit2 = Physics2D.Raycast(muzzlePos.position, new Vector2(-1, 0), aimDistance, mask);
        else
            rayHit2 = Physics2D.Raycast(muzzlePos.position, new Vector2(1, 0), aimDistance, mask);
        
        

        if(rayHit2.collider != null && rayHit2.collider.CompareTag(playerTag) && isSnipingCoolTime == false && isAttackCoolTime == false)     //현재 바라보는 방향 일직선에서 플레이어가 감지되면
        {
            isAiming = true;
            am.SetBool("Aim", true);        //조준 애니메이션
            aimLaser.SetActive(true);       //레이저 오브젝트 활성화

            StartCoroutine(AimingPlayer());
            StartCoroutine(SnipingCoolTime());
            StartCoroutine(AttackCoolTime());       //공격 쿨타임

            return;
        }

         
    }




    //---------------잡기 공격------------------
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
        StartCoroutine(AttackCoolTime());       //공격 쿨타임
    }


    public void GiveGrabDamage()       //잡기 공격 에니메이션에서 실행됨
    {
        //SoundManager.instance.EnemySfxSound(attackAudio,"HeadMachineShoot");

        Collider2D collider = Physics2D.OverlapBox(grabScope.position,grabScope.localScale,0,playerLayer);
        
        if(collider != null)    //플레이어가 있으면
        {   
            //ITakeDamage 인터페이스를 가진 대상으로 인터페이스 함수(TakeDamage) 실행
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, grabAttackPower);     //대상의 TakeDamage 함수 실행
        }
    }


    //-----------------------돌진 공격-------------------------

    IEnumerator Charging()  //플레이어 조준
    {
        for(int i=0; i<10; i++)  //3초 대기
        {
            yield return chargingWaitTime;
        }

        if(isCharging == true)  //돌진
        {
            isCharging = false;
            isCrushing = true;
            
            am.SetBool("Crush",true);
            am.SetBool("Charge", false);
            StartCoroutine("Crushing");
        }
      
    }

    IEnumerator Crushing()
    {
        
        for(int i=0; i<15; i++)
        {
            if(transform.localScale.x > 0)  //왼쪽 볼 때
            {
                transform.Translate(new Vector2(-1 * 0.3f ,0));
                GiveCrushDamage();
            }
            else
            {
                transform.Translate(new Vector2(1 * 0.3f ,0));
                GiveCrushDamage();
            }
            

            yield return crushMotionTime;
        }
        isCrushing = false;
        am.SetBool("Crush",false);
        StartCoroutine(AttackCoolTime());       //공격 쿨타임
    }

    public void GiveCrushDamage()       //돌진 공격 데미지 주는 함수
    {
        //SoundManager.instance.EnemySfxSound(attackAudio,"HeadMachineShoot");

        Collider2D collider = Physics2D.OverlapBox(crushHitBox.position,crushHitBox.localScale,0,playerLayer);
        
        if(collider != null)    //플레이어가 있으면
        {   
            //ITakeDamage 인터페이스를 가진 대상으로 인터페이스 함수(TakeDamage) 실행
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, crushAttackPower);     //대상의 TakeDamage 함수 실행
        }
    }



    //-----------------------원거리 공격-------------------------
    IEnumerator AimingPlayer()  //플레이어 조준
    {
        for(int i=0; i<10; i++)
        {
            //플레이어가 공격 범위 안에 있는지 0.3초 단위로 반복 검사
            RaycastHit2D rayHit;
            if(transform.localScale.x > 0)
                rayHit = Physics2D.Raycast(muzzlePos.position, new Vector2(-1, 0), aimDistance, mask);
            else
                rayHit = Physics2D.Raycast(muzzlePos.position, new Vector2(1, 0), aimDistance, mask);
        
            
            if(isAiming == false || rayHit.collider == null || !rayHit.collider.CompareTag(playerTag))        //피격 시 isAiming => false  -> 조준 해제
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
            am.SetTrigger("Shoot");
            am.SetBool("Aim", false);
        }

        StartCoroutine(AttackCoolTime());       //공격 쿨타임
      
    }




    void Shoot()        //조준 후, 발사 Shoot 에니메이션에서 발동 (에니메이션 이벤트 함수)
    {
        //SoundManager.instance.EnemySfxSound(attackAudio,"ArmMachineShoot");
        
        //------임시로 ArmMachine 총알 사용 
        AmBullet bullet = Instantiate(bulletPrefeb, muzzlePos.position, muzzlePos.rotation).GetComponent<AmBullet>();
        if(transform.localScale.x > 0)    //왼쪽 보고 있으면
        {
            bullet.SetBullet(leftDirection);
        }
        else    //왼쪽 보고 있으면
        {
            bullet.SetBullet(rightDirection);
        }
    }




    //-----------움직임 방향 설정 관련---------------
    void SetMoveDirection()     //몬스터 움직임 방향을 3초마다 설정
    {
        nextMove = Random.Range(-1,2);      //-1~1까지 

        int time = Random.Range(1,5);
        Invoke("SetMoveDirection", time);       //다음 방향 설정
    }

    public void TakeDamage(Transform attacker, int damage)
    {
        /* 사운드
        if(Random.Range(0,2) == 0)
            SoundManager.instance.EnemySfxSound(behaviorAudio,"HeadMachineHit1");
        else
            SoundManager.instance.EnemySfxSound(behaviorAudio,"HeadMachineHit2");
        */

        //잡는 도중에 맞으면 잡기 상태 풀고, 플레이어도 홀딩 상태 해제
        if(isGrab == true)
        {
            isGrab = false;
            am.SetBool("Grab", false);

            player.isHolding = false;
            player.am.SetBool("Holding",false);
        }
        //-------

        if(isCharging == true || isCrushing == true)        //돌진 공격 때는 무적
        {
            return;
        }

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
                //SoundManager.instance.EnemySfxSound(behaviorAudio,"HeadMachineDie");

                currentHp = 0;
                isDead = true;
                //am.SetTrigger("Dead");

                table.GetComponent<TableEnding>().enabled = true;
                table.GetComponent<BoxCollider2D>().enabled = true;

                Destroy(gameObject, 2f);        //2초 후 사라짐
            }
        }
    }




    //------------쿨타임 관련-------------

    IEnumerator AttackCoolTime()
    {
        isAttackCoolTime = true;
        Debug.Log("공격 기본 쿨타임 on");
        yield return attackCoolTime;

        isAttackCoolTime = false;
        Debug.Log("공격 기본 쿨타임 off");
    }

    IEnumerator GrabCoolTime()
    {
        isGrabCoolTime = true;

        yield return grabCoolTime;

        isGrabCoolTime = false;
    }


    IEnumerator SnipingCoolTime()
    {
        isSnipingCoolTime = true;

        yield return shootCoolTime;

        isSnipingCoolTime = false;
    }

    IEnumerator CrushCoolTime()
    {
        isCrushCoolTime = true;

        yield return crushCoolTime;

        isCrushCoolTime = false;
    }




    void SetLimits()
    {
        minX = mapBoundary.transform.position.x - mapBoundary.bounds.size.x * 0.5f;
        maxX = mapBoundary.transform.position.x + mapBoundary.bounds.size.x * 0.5f;
        minY = mapBoundary.transform.position.y - mapBoundary.bounds.size.y * 0.5f;
        maxY = mapBoundary.transform.position.y + mapBoundary.bounds.size.y * 0.5f;
    }


    //-------히트박스 나타내기-------(인게임에서는 안보임) (히트박스 크기 조절하기 위해 만든 함수)
    private void OnDrawGizmos()     
    {                               
        Gizmos.color = Color.red;
       // Gizmos.DrawWireCube(grabScope.position,grabScope.localScale);
        Gizmos.DrawWireCube(crushHitBox.position,crushHitBox.localScale);
    }



    void TestDrawRay()
    {
        if(transform.localScale.x > 0)  //왼쪽 보고 있을 때
        {
            Debug.DrawRay(muzzlePos.position,Vector2.left * trackingDistance, Color.yellow);
            Debug.DrawRay(muzzlePos.position,Vector2.left * aimDistance, Color.red);
            Debug.DrawRay(muzzlePos.position,Vector2.left * crushDistance, Color.blue);
        }
        else
        {
            Debug.DrawRay(muzzlePos.position,Vector2.right * trackingDistance, Color.yellow);
            Debug.DrawRay(muzzlePos.position,Vector2.right * aimDistance, Color.red);
            Debug.DrawRay(muzzlePos.position,Vector2.right * crushDistance, Color.blue);
        }
        
    }
}
