using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

//------------------튜토리얼 전용 player 스크립트 (수정주의)-------------------
public class TutorialPlayer : MonoBehaviour, ITakeDamage
{
    [SerializeField]
    private TutorialManager tm;
    
    public PlayerUICanvas playerUi;

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


    //------인터페이스 관련-------
    public bool isInterface = false;

    //-------튜토리얼 관련--------
    public bool clearCheck = false;
    public PlayableDirector monsterTimeline;
    bool isPlayTimeline = false;
    public GameObject timelineCamera;

    public bool isDownAttack = false;
    public bool isUpAttack = false;
    public bool isThrust = false;
    public bool isShoot = false;


    //-------대화 관련------
    public ConversationObject conversationInfo;

   
    public void Awake()
    {
        playerUi = GameObject.Find("PlayerUICanvas").GetComponent<PlayerUICanvas>();
        attackAudio = transform.Find("AttackAudio").GetComponent<AudioSource>();
        behaviorAudio = transform.Find("BehaviorAudio").GetComponent<AudioSource>();

        mapBoundary = GameObject.Find("MapBoundary").GetComponent<BoxCollider2D>();
    }

    public void Start()
    {
        maxHp = 10000;
        currentHp = 10000;
        speed = 5.0f;
        jumpPower = 5.0f;
        currentBulletCnt = 3;

        playerUi.SetPlayerHpBar(currentHp);
        playerUi.SetPlayerBulletCount(currentBulletCnt);

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
        if(monsterTimeline.gameObject.activeSelf == true) 
        {
            am.SetBool("Run",false);
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
        if(Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.RightArrow))   //왼쪽 달리기
        {     
            transform.localScale = lScale;
            am.SetBool("Run",true);
            playerDirection = 0;

            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        else if(Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.LeftArrow))    //오른쪽 달리기
        {
            //-----왼쪽 달리기 튜토리얼 못 깼으면 이동안됨-----
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.left] == false)
                return;             

            transform.localScale = rScale;  //-----캐릭터 방향 처리-----
            am.SetBool("Run",true);     //달리는 에니메이션
            playerDirection = 1;

            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        else if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))   //가만히 있을 때
        {
            am.SetBool("Run",false);
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

            //-----오른쪽 달리기 튜토리얼 못 깼으면 점프안됨-----
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.right] == false)
                return;         


            if(!IsJump())       //현재 공중에 있지 않으면 (땅에 닿아있으면)
            {
                SoundManager.instance.PlayerSfxSound(behaviorAudio,"PlayerJump");
                rb.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);       //(impulse => 순간적으로 힘을 준다)
            }
        }

        //---------인터페이스(대화) 관련-------

        if(isInterface == true && Input.GetKeyDown(KeyCode.G))
        {
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.jump] == false)
                return;


            if(conversationInfo != null)        //인터페이스 가능한 오브젝트가 대화 가능 오브젝트이면
            {
                playerUi.StartDialog(conversationInfo.speaker, conversationInfo.content);

                if(conversationInfo.eventName.Equals("Tutorial1Clear"))
                {
                    StartCoroutine(tm.ClearTutorial1());
                }
            }
            
            
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.conversation] == false && clearCheck == false)
            {
                tm.ClearCurrentTutorial(tm.currentStage);
                clearCheck = true;  //한 번만 실행되도록 만든 임시 bool 변수
            }

        }


        //----------플레이어 공격 관련-----
        if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.W) && isAttack == false)
        {
            //-----down 공격 튜토리얼 못 깼으면 공격 안됨-----
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.downAttack] == false)
                return;      

            isAttack = true;
            isUpAttack = true;
            am.SetTrigger("UpAttack");

        }
        else if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.E) && isAttack == false)
        {
            //-----up 공격 튜토리얼 못 깼으면 공격 안됨-----
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.upAttack] == false)
                return;      

            isAttack = true;
            isThrust = true;
            am.SetTrigger("ThrustAttack");

        }
        else if(Input.GetMouseButtonDown(0) && isAttack == false)
        {
            //-----상호작용 튜토리얼 못 깼으면 공격 안됨-----
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.conversation] == false)
                return;       

            isAttack = true;
            isDownAttack = true;
            am.SetTrigger("DownAttack");

        }
        else if(Input.GetMouseButtonDown(1) && currentBulletCnt > 0 && isAttack == false)
        {   
            //-----찌르기 공격 튜토리얼 못 깼으면 공격 안됨-----
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.thrust] == false)
                return;


            isAttack = true;
            isShoot = true;
            am.SetTrigger("ShootAttack");

            /*
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.shoot] == false && clearCheck == false)
            {
                tm.ClearCurrentTutorial(tm.currentStage);
                clearCheck = true;  //한 번만 실행되도록 만든 임시 bool 변수
            }
            */
        }
       

    }

    void DownAttack()
    {
        SoundManager.instance.PlayerSfxSound(attackAudio,"PlayerDownAttack");
        Collider2D collider = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,1 << LayerMask.NameToLayer("Enemy"));
        
        if(collider != null)    //적이 있으면
        {   
            //ITakeDamage 인터페이스를 가진 대상으로 인터페이스 함수(TakeDamage) 실행
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, 15);     //대상의 TakeDamage 함수 실행
        }
        isAttack = false;
        isDownAttack = false;
    }

    void UpAttack()
    {
        SoundManager.instance.PlayerSfxSound(attackAudio,"PlayerUpAttack");
        Collider2D collider = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,1 << LayerMask.NameToLayer("Enemy"));
        
        if(collider != null)    //적이 있으면
        {   
            //ITakeDamage 인터페이스를 가진 대상으로 인터페이스 함수(TakeDamage) 실행
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, 20);     //대상의 TakeDamage 함수 실행
        }
        isAttack = false;
        isUpAttack = false;
    }

    void ThrustAttack()
    {
        SoundManager.instance.PlayerSfxSound(attackAudio,"PlayerThrustAttack");
        Collider2D collider = Physics2D.OverlapBox(hitBox.position,hitBox.localScale,0,1 << LayerMask.NameToLayer("Enemy"));
        
        if(collider != null)    //적이 있으면
        {   
            //ITakeDamage 인터페이스를 가진 대상으로 인터페이스 함수(TakeDamage) 실행
            collider.GetComponent<ITakeDamage>().TakeDamage(this.transform, 20);     //대상의 TakeDamage 함수 실행
        }
        isAttack = false;
        isThrust = false;
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
        RaycastHit2D rayHit = Physics2D.Raycast(currentVec,Vector2.down,1f,LayerMask.GetMask("Block"));

        if(rayHit.collider == null)
            return true;
        else
            return false;
    }

    
    void OnTriggerEnter2D(Collider2D other) 
    {
        //----대화 관련 상태----
        if(other.gameObject.tag.Equals("ConversationObject"))
        {
            isInterface = true;

            conversationInfo = other.GetComponent<ConversationObject>();
            tm.SetKeyTutorial(TutorialManager.tutorialStage.conversation);
        }


        if(other.gameObject.tag.Equals("InterfaceObject"))
        {
            isInterface = true;
            
            tm.SetKeyTutorial(TutorialManager.tutorialStage.conversation);
        }


        //------움직임 튜토리얼 도달 위치-------
        if(other.gameObject.name == "A_Location")
        {
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.left] == false && clearCheck == false)
            {
                tm.ClearCurrentTutorial(tm.currentStage);
                clearCheck = true;  //한 번만 실행되도록 만든 임시 bool 변수
            }
        }

        if(other.gameObject.name == "D_Location")
        {
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.right] == false && clearCheck == false)
            {
                tm.ClearCurrentTutorial(tm.currentStage);
                clearCheck = true;  //한 번만 실행되도록 만든 임시 bool 변수
            }
        }


        if(other.gameObject.name == "Space_Location")
        {
            if(tm.clearStaus[(int)TutorialManager.tutorialStage.jump] == false && clearCheck == false)
            {
                tm.ClearCurrentTutorial(tm.currentStage);
                clearCheck = true;  //한 번만 실행되도록 만든 임시 bool 변수
            }
        }
        
        if(other.gameObject.name == "DollyCam_Location")
        {
            if(isPlayTimeline == false)
            {
                monsterTimeline.gameObject.SetActive(true);
                monsterTimeline.Play();
                isPlayTimeline = true;
            }
            
        }
    }


    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag.Equals("InterfaceObject"))
        {
            isInterface = false;
            tm.keyTutorial.gameObject.SetActive(false);
        }   


        if(other.gameObject.tag.Equals("ConversationObject"))
        {
            isInterface = false;
            conversationInfo = null;
        }
    }



    public void TakeDamage(Transform attacker, int damage)
    {

        if(Random.Range(0,2) == 0)
            SoundManager.instance.PlayerSfxSound(behaviorAudio,"PlayerHit1");
        else
            SoundManager.instance.PlayerSfxSound(behaviorAudio,"PlayerHit2");
        

        if(isAttack == true)
            isAttack = false;


        currentHp = currentHp - 0;
        playerUi.SetPlayerHpBar(currentHp);


        if(currentHp > 0)      //히트
        {
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


    public void ExitTimeline()
    {
        timelineCamera.SetActive(false);
        monsterTimeline.gameObject.SetActive(false);
    }
}
