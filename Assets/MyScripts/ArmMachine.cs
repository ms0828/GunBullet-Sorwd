using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmMachine : Enemy, ITakeDamage
{
    //-------스탯 관련-------
    public int knifePower = 5;
    public int shootPower = 50;
    public float aimDistance = 7f;


    //------상태 관련 bool-------
    public bool isknockback = false;
    public bool isKnifeCoolTime = false;
    public bool isSnipingCoolTime = false;
    public bool isAiming = false;


    //------필요 Object or Transform------
    public Transform muzzlePos;
    public GameObject aimLaser;
    public GameObject bulletPrefeb;
    public Transform knifeScope;    //칼 공격 범위


    //------쿨타임 코루틴 캐싱------
    public WaitForSeconds knifeCoolTime = new WaitForSeconds(3.0f);
    public WaitForSeconds shootCoolTime = new WaitForSeconds(7.0f);


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
    }
    */
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


    void ObservePlayer()       //플레이어 감지
    {
        if(isAiming == true)    //현재 조준 중이면 함수 종료
            return;


        //-----적이 가까이 오면 근접 공격 발동------
        Collider2D collider = Physics2D.OverlapBox(knifeScope.position,knifeScope.localScale,0,playerLayer);

        if(collider != null && isKnifeCoolTime == false)    // 공격 범위 내에 플레이어가 있으면 
        {   
            am.SetTrigger("KnifeAttack");
            StartCoroutine(KnifeCoolTime());

            return;
        }
        //------------------------------------------



        //-----적이 멀리있으면 원거리 저격 조준------
        RaycastHit2D rayHit;
        if(transform.localScale.x > 0)
            rayHit = Physics2D.Raycast(muzzlePos.position, new Vector2(1, 0), aimDistance, mask);
        else
            rayHit = Physics2D.Raycast(muzzlePos.position, new Vector2(-1, 0), aimDistance, mask);
        
        

        if(rayHit.collider != null && rayHit.collider.CompareTag(playerTag) && isSnipingCoolTime == false)     //현재 바라보는 방향 일직선에서 플레이어가 감지되면
        {
            isAiming = true;
            am.SetBool("Aim", true);        //조준 애니메이션
            aimLaser.SetActive(true);       //레이저 오브젝트 활성화

            StartCoroutine(AimingPlayer());
            StartCoroutine(SnipingCoolTime());
        }

    }

    //-------------근접 공격 관련--------------
    void KnifeAttack()
    {
        SoundManager.instance.EnemySfxSound(attackAudio,"ArmMachineKnife");

        Collider2D collider = Physics2D.OverlapBox(knifeScope.position,knifeScope.localScale,0,playerLayer);
        
        if(collider != null)    //플레이어가 있으면
        {   
            //ITakeDamage 인터페이스를 가진 대상으로 인터페이스 함수(TakeDamage) 실행
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, knifePower);     //대상의 TakeDamage 함수 실행
        }
    }

    //근거리 공격 후 백스탭
    void BackStep()
    {
        am.SetTrigger("BackStep");
        if(transform.position.x - player.transform.position.x > 0)    // 플레이어 위치의 반대 방향으로 넉백                    
            rb.AddForce(new Vector2(3f,2f), ForceMode2D.Impulse);
        else
            rb.AddForce(new Vector2(-3f,2f), ForceMode2D.Impulse);
    }

    IEnumerator AimingPlayer()  //플레이어 조준
    {
        for(int i=0; i<10; i++)
        {
            //플레이어가 공격 범위 안에 있는지 0.3초 단위로 반복 검사
            RaycastHit2D rayHit;
            if(transform.localScale.x > 0)
                rayHit = Physics2D.Raycast(muzzlePos.position, new Vector2(1, 0), aimDistance, LayerMask.GetMask("Player"));
            else
                rayHit = Physics2D.Raycast(muzzlePos.position, new Vector2(-1, 0), aimDistance, LayerMask.GetMask("Player"));
        

            if(isAiming == false || rayHit.collider == null)        //피격 시 isAiming => false  -> 조준 해제
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
      
    }


    void Shoot()        //조준 후, 발사 Shoot 에니메이션에서 발동 (에니메이션 이벤트 함수)
    {
        SoundManager.instance.EnemySfxSound(attackAudio,"ArmMachineShoot");

        AmBullet bullet = Instantiate(bulletPrefeb, muzzlePos.position, muzzlePos.rotation).GetComponent<AmBullet>();
        if(transform.localScale.x > 0)    //오른쪽 보고 있으면
        {
            bullet.SetBullet(rightDirection);
        }
        else    //왼쪽 보고 있으면
        {
            bullet.SetBullet(leftDirection);
        }
    }






    // ----- 쿨타임 관련 -----
    IEnumerator KnifeCoolTime()
    {
        isKnifeCoolTime = true;

        yield return knifeCoolTime;

        isKnifeCoolTime = false;
    }
    IEnumerator SnipingCoolTime()
    {
        isSnipingCoolTime = true;

        yield return shootCoolTime;

        isSnipingCoolTime = false;
    }


    public void TakeDamage(Transform attacker, int damage)
    {  
        //조준 도중에 맞으면 조준 상태 해제
        if(isAiming == true)
            isAiming = false;
        

        if(Random.Range(0,2) == 0)
            SoundManager.instance.EnemySfxSound(behaviorAudio,"ArmMachineHit1");
        else
            SoundManager.instance.EnemySfxSound(behaviorAudio,"ArmMachineHit2");



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
                SoundManager.instance.EnemySfxSound(behaviorAudio,"ArmMachineDie");

                currentHp = 0;
                isDead = true;
                //am.SetTrigger("Dead");

                GetComponent<Collider2D>().enabled = false;
                Destroy(GetComponent<Rigidbody2D>());
                Destroy(gameObject, 1f);        //2초 후 사라짐
            }
        }
    }

    
}