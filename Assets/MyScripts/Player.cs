using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, ITakeDamage
{
    public PlayerUICanvas playerUi;

    public SpriteRenderer sr;
    public Rigidbody2D rb;
    public Animator am;

    public Transform hitBox;    //공격 범위

    //-----스탯 관련-----
    public int maxHp;
    public int currentHp;

    public float speed;
    public float jumpPower;

    //-----상태 관련-----
    public bool isDead = false;
    public bool isHolding = false;
    public bool isJumpCoolTime = false;
    public WaitForSeconds jumpDelayTime = new WaitForSeconds(0.5f);



    //-----움직임 방향 관련----
    private Vector3 lScale = new Vector3(-3,3,1);
    private Vector3 rScale = new Vector3(3,3,1);
    private Vector2 leftDirection = new Vector2(-1,0);
    private Vector2 rightDirection = new Vector2(1,0);
    public int playerDirection = 1;     //오른쪽 보고 있을 때 1, 왼쪽 보고 있을 때 0


    //----움직임 제한----
    float minX, maxX, minY, maxY;

    BoxCollider2D mapBoundary;
    

    //-----사격 관련-----
    public Transform muzzlePos;
    public GameObject bulletPrefeb;
    public int maxBulletCnt = 3;
    public int currentBulletCnt;
    public WaitForSeconds reloadTime = new WaitForSeconds(3.0f);

    public bool isAttack = false;

    
    //-----잡기 상태 관련-------
    public int holdingGauge = 0;



    //---------사운드 관련----------
    public AudioSource attackAudio;
    public AudioSource behaviorAudio;


    
    //--------인터페이스 관련--------
    public bool isInterface = false;
    public ConversationObject conversationInfo;
    public IInterfaceObject interfaceInfo;
    public IndicationKey indicationKey;


    //------피격 딜레이------
    public WaitForSeconds hitDelayTime = new WaitForSeconds(0.2f);
    public bool isHitDelay = false;

    public Color normalColor;
    public Color hitColor;
   

    //--------공격 콤보 관련---------
    public WaitForSeconds nextAttackDelay = new WaitForSeconds(0.2f);
    int currentAttack = 0;
    bool isNextAttack = false;



    public void Awake()
    {
      
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

        if(indicationKey == null)
        {
            indicationKey = transform.Find("IndicationKey").GetComponent<IndicationKey>();
        }
        
    }

    public void Start()
    {
        maxHp = 200;
        currentHp = 200;
        speed = 5.0f;
        jumpPower = 5.0f;
        currentBulletCnt = 3;
        currentAttack = 0;


        playerUi.SetPlayerHpBar(currentHp);
        playerUi.SetPlayerBulletCount(currentBulletCnt);

        normalColor = new Color(1f,1f,1f,1f);
        hitColor = new Color(1f,1f,1f,0.7f);

        SetLimits();    //움직임 영역 제한 설정
    }

    public void Update()
    {
        if(isDead)
            return;

        PlayerControll();
    }

    void LateUpdate()   //움직임 영역 제한
    {
        float xClamp = Mathf.Clamp(transform.position.x, minX, maxX);
        float yClamp = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(xClamp, yClamp, transform.position.z);
    }



    public void PlayerControll()
    {
        if(playerUi.isPlayTimeline == true)
            return;

        
        
        if(isHolding)       //홀딩 상태면 컨트롤 불가
        {
            if(Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.D))
            {
                holdingGauge += 10;
            }

            return;
        }


        //-----플레이어 대화------
        if(playerUi.dialogBox.activeSelf == true)
        {
            if(Input.anyKeyDown && playerUi.isPrintingDialog == false)
            {
                playerUi.NextDialog();
            }
               
            return;
        }
        

        //---------플레이어 좌우 움직임-----------
        if(Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.LeftArrow))    //오른쪽 달리기
        {
            transform.localScale = rScale;  //-----캐릭터 방향 처리-----
            am.SetBool("Run",true);     //달리는 에니메이션
            playerDirection = 1;

            
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));        
        }
        else if(Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.RightArrow))   //왼쪽 달리기
        {     
            transform.localScale = lScale;  
            am.SetBool("Run",true);
            playerDirection = 0;

            
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        else if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))   //가만히 있을 때
        {
            am.SetBool("Run",false);
        }




        //---------인터페이스(대화) 관련-------

        if(isInterface == true && Input.GetKeyDown(KeyCode.G))
        {
            if(conversationInfo != null)        //인터페이스 가능한 오브젝트가 대화 가능 오브젝트이면
            {
                playerUi.StartDialog(conversationInfo);
            }
            else if(interfaceInfo != null)
            {
                indicationKey.gameObject.SetActive(false);
                interfaceInfo.Interface();
            }
        }

        


        //----------플레이어 점프----

        if(IsJump())        //(공중에 떠있으면 점프 에니메이션 ON) 
        {
            am.SetBool("Jump",true);
        }
        else
        {
            am.SetBool("Jump",false);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!IsJump() && isJumpCoolTime == false)       //현재 공중에 있지 않으면 (땅에 닿아있으면)
            {
                StartCoroutine(JumpCoolTime());
                SoundManager.instance.PlayerSfxSound(behaviorAudio,"PlayerJump");
                rb.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);       //(impulse => 순간적으로 힘을 준다)
            }
        }



        //----------플레이어 공격 관련-----
        if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.W) && isAttack == false)
        {
            isAttack = true;
            am.SetTrigger("UpAttack");
        }
        else if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.E) && isAttack == false)
        {
            isAttack = true;
            am.SetTrigger("ThrustAttack");
        }
        else if(Input.GetMouseButtonDown(0) && isAttack == false)
        {
            isAttack = true;

            if(currentAttack == 0)
            {
                am.SetTrigger("DownAttack");
                currentAttack = 1;
                isNextAttack = true;
                StartCoroutine(NextAttack());
            }
            else if(currentAttack == 1)
            {
                am.SetTrigger("UpAttack");
                currentAttack = 2;
                isNextAttack = true;
                StartCoroutine(NextAttack());
            }
            else if(currentAttack == 2)
            {
                am.SetTrigger("ThrustAttack");
                currentAttack = 0;
            }
            
        }
        else if(Input.GetMouseButtonDown(1) && currentBulletCnt > 0 && isAttack == false)
        {   
            isAttack = true;
            am.SetTrigger("ShootAttack");
        }
       

    }

    void DownAttack()
    {
        SoundManager.instance.PlayerSfxSound(attackAudio,"PlayerDownAttack");
        Collider2D collider = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,1 << LayerMask.NameToLayer("Enemy"));
        isAttack = false;

        if(collider != null)    //적이 있으면
        {   
            //ITakeDamage 인터페이스를 가진 대상으로 인터페이스 함수(TakeDamage) 실행
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, 50);     //대상의 TakeDamage 함수 실행
        }

        isNextAttack = false;
        
    }

    void UpAttack()
    {
        SoundManager.instance.PlayerSfxSound(attackAudio,"PlayerUpAttack");
        Collider2D collider = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,1 << LayerMask.NameToLayer("Enemy"));
        isAttack = false;

        if(collider != null)    //적이 있으면
        {   
            //ITakeDamage 인터페이스를 가진 대상으로 인터페이스 함수(TakeDamage) 실행
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, 50);     //대상의 TakeDamage 함수 실행
        }
        
        isNextAttack = false;
    }

    void ThrustAttack()
    {
        SoundManager.instance.PlayerSfxSound(attackAudio,"PlayerThrustAttack");
        Collider2D collider = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,1 << LayerMask.NameToLayer("Enemy"));
        isAttack = false;
        
        if(collider != null)    //적이 있으면
        {   
            //ITakeDamage 인터페이스를 가진 대상으로 인터페이스 함수(TakeDamage) 실행
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, 50);     //대상의 TakeDamage 함수 실행
        }
        
    }


    void Shooting()     //포격 에니메이션에서 호출됨 (에니메이션 이벤트 함수)   -> 총알 발사
    {
        SoundManager.instance.PlayerSfxSound(attackAudio,"PlayerShoot", 0.4f);

        Bullet bullet = Instantiate(bulletPrefeb, muzzlePos.position, muzzlePos.rotation).GetComponent<Bullet>();
        if(playerDirection == 1)    //오른쪽 보고 있으면
        {
            bullet.SetBullet(rightDirection);
        }
        else    //왼쪽 보고 있으면
        {
            bullet.SetBullet(leftDirection);
        }
        
        currentBulletCnt--;
        playerUi.SetPlayerBulletCount(currentBulletCnt);

        if(currentBulletCnt <= 0)
        {
            StartCoroutine(Reload());
        }

        isAttack = false;
    }

    IEnumerator Reload()
    {
        yield return reloadTime;

        SoundManager.instance.PlayerSfxSound(attackAudio,"PlayerReload", 1f);
        currentBulletCnt = 3;
        playerUi.SetPlayerBulletCount(currentBulletCnt);
    }


    bool IsJump()       //캐릭터가 공중에 떠있는지 확인하는 함수
    {
        Vector2 currentVec = transform.position;
        RaycastHit2D rayHit = Physics2D.Raycast(currentVec,Vector2.down,0.6f,LayerMask.GetMask("Block"));

        if(rayHit.collider == null)
            return true;
        else
            return false;
    }



    /*
    void OnTriggerStay2D(Collider2D other) 
    {
        if(other.gameObject.tag.Equals("Portal"))   //포탈
        {
            if(Input.GetKeyDown(KeyCode.W))
            {
                GameManager.instance.currentStage++;
                GameManager.instance.SaveUserData();
                SceneManager.LoadScene(GameManager.instance.currentStage);
            }
        }
    }*/

    void OnTriggerEnter2D(Collider2D other) 
    {
        //----대화 관련 상태----
        if(other.gameObject.tag.Equals("ConversationObject"))
        {
            isInterface = true;

            conversationInfo = other.GetComponent<ConversationObject>();
            indicationKey.gameObject.SetActive(true);
            indicationKey.SetKeyInterface();
            
        }


        if(other.gameObject.tag.Equals("InterfaceObject"))
        {
            isInterface = true;
            
            interfaceInfo = other.GetComponent<IInterfaceObject>();
            indicationKey.gameObject.SetActive(true);
            indicationKey.SetKeyInterface();
        }

    }


    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag.Equals("InterfaceObject"))
        {
            isInterface = false;
            interfaceInfo = null;
            indicationKey.gameObject.SetActive(false);
        }   


        if(other.gameObject.tag.Equals("ConversationObject"))
        {
            isInterface = false;
            conversationInfo = null;
            indicationKey.gameObject.SetActive(false);
        }
    }


    public void TakeDamage(Transform attacker, int damage)
    {

        if(isHolding == true)
        {
            isHolding = false;      //피격 시, 홀딩 해제
            am.SetBool("Holding",false);
            indicationKey.gameObject.SetActive(false);
        }
        if(isAttack == true)
            isAttack = false;

        

        if(isHitDelay == true)      //피격시 2초간 무적상태
        {
            return;
        }


        if(Random.Range(0,2) == 0)
            SoundManager.instance.PlayerSfxSound(behaviorAudio,"PlayerHit1");
        else
            SoundManager.instance.PlayerSfxSound(behaviorAudio,"PlayerHit2");
        
        
        currentHp = currentHp - damage;
        playerUi.SetPlayerHpBar(currentHp);

        if(currentHp > 0)      //히트
        {
            am.SetTrigger("Hit");

            StartCoroutine("HitDelay");

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
                SoundManager.instance.PlayerSfxSound(behaviorAudio,"PlayerDie");

                currentHp = 0;
                isDead = true;
                //am.SetTrigger("Dead");

                playerUi.deadImage.SetActive(true);
                Destroy(gameObject, 2f);        //2초 후 사라짐
            }
        }
    }


    public IEnumerator Holding(Enemy attacker)   //HeadMachine이 잡기 공격을 했을 때 실행되는 함수
    {
        isHolding = true;
        am.SetBool("Holding",true);
        holdingGauge = 0;

        indicationKey.gameObject.SetActive(true);
        int j = 0;
        
        if(attacker is HeadMachine)
        {
            HeadMachine headMachine = (HeadMachine)attacker;

            
            for(int i=0; i<20; i++)
            {
                if(j==0)
                {
                    indicationKey.am.SetTrigger("A");
                    j = 1;
                }
                else
                {
                    indicationKey.am.SetTrigger("D");
                    j = 0;
                }
                
                if(holdingGauge >= 100 || isHolding == false || headMachine.isGrab == false)       //게이지 다채우거나, 잡는 적이 피격 시 홀딩해제
                {
                    am.SetBool("Holding",false);
                    isHolding = false;

                    if(holdingGauge >= 100)
                        headMachine.TakeDamage(this.transform, 0);        //게이지 채워서 탈출 성공하면 적 밀쳐냄

                    indicationKey.gameObject.SetActive(false);
                    yield break;
                }

                holdingGauge -= 10;
                if(holdingGauge <= 0)
                    holdingGauge = 0;


                yield return new WaitForSeconds(0.2f);
            }
        }



        if(attacker is BossMonster)
        {
            BossMonster bossMonster = (BossMonster)attacker;

            for(int i=0; i<15; i++)
            {

                if(j==0)
                {
                    indicationKey.am.SetTrigger("A");
                    j = 1;
                }
                else
                {
                    indicationKey.am.SetTrigger("D");
                    j = 0;
                }


                if(holdingGauge >= 100 || isHolding == false || bossMonster.isGrab == false)       //게이지 다채우거나, 잡는 적이 피격 시 홀딩해제
                {
                    am.SetBool("Holding",false);
                    isHolding = false;

                    if(holdingGauge >= 100)
                        bossMonster.TakeDamage(this.transform, 0);        //게이지 채워서 탈출 성공하면 적 밀쳐냄

                    indicationKey.gameObject.SetActive(false);
                    yield break;
                }

                holdingGauge -= 10;
                if(holdingGauge <= 0)
                    holdingGauge = 0;


                yield return new WaitForSeconds(0.2f);
            }
        }
        
    }




    IEnumerator HitDelay()
    {
        isHitDelay = true;

        int j = 0;

        for(int i=0; i<10; i++)
        {
            if(j==0)
            {
                sr.color = hitColor;
                j = 1;
            }
            else
            {
                sr.color = normalColor;
                j = 0;
            }
            
            yield return hitDelayTime;
        }
        
        isHitDelay = false;
    }


    IEnumerator JumpCoolTime()
    {
        isJumpCoolTime = true;

        yield return jumpDelayTime;

        isJumpCoolTime = false;
    }

    
    IEnumerator NextAttack()
    {
        for(int i=0; i<20; i++)
        {
            if(i > 10 && isNextAttack == true)
            {
                yield break;
            }
            yield return nextAttackDelay;
        }
        currentAttack = 0;
    }

    //-------히트박스 나타내기-------(인게임에서는 안보임) (히트박스 크기 조절하기 위해 만든 함수)
    private void OnDrawGizmos()     
    {                               
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hitBox.position,hitBox.localScale);
    }


    void SetLimits()
    {
        minX = mapBoundary.transform.position.x - mapBoundary.bounds.size.x * 0.5f;
        maxX = mapBoundary.transform.position.x + mapBoundary.bounds.size.x * 0.5f;
        minY = mapBoundary.transform.position.y - mapBoundary.bounds.size.y * 0.5f;
        maxY = mapBoundary.transform.position.y + mapBoundary.bounds.size.y * 0.5f;
    }


}
