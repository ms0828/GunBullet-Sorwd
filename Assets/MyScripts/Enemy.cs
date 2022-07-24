using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, ITakeDamage    //적 추상클래스 (적은 이 클래스를 상속받아 추상 함수 구현해야함)
{
    public int maxHp;
    public int currentHp;
    public int shortAttackPower;
    public int longAttackPower;

    public float speed;

    //-----기본 컴포넌트-----
    public Animator am;            // 에니메이터
    private Rigidbody2D rb;         // Rigidbody
    private SpriteRenderer sr;      //스프라이트 렌더러 (x축 filp 설정 위해)
    public Transform hitBox;    //공격 범위    (자식으로 빈 오브젝트(히트박스) 크기 설정한다음 인스펙터로 할당해주세요)


    //-----상태 관련 bool-----
    private bool isDead = false;
    

    //-----움직임 방향 관련----
    public int nextMove = 0;       //0 정지, -1 왼쪽, 1 오른쪽
    private Vector3 leftDirection = new Vector3(-3,3,1);     //왼쪽으로 갈 때 몬스터에게 크기 할당(이미지 좌우 반전)
    private Vector3 rightDirection = new Vector3(3,3,1);    //오른쪽으로 갈 때




    public abstract void ShortAttack();     //근접 공격

    public abstract void LongAttack();      //원거리 공격


    public void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>(); //스프라이트 렌더러 가져오기
        rb = gameObject.GetComponent<Rigidbody2D>();   //Rigidbody 가져오기
        am = gameObject.GetComponent<Animator>();       //에니메이터 가져오기
    }

    public void Start()
    {
        Invoke("SetMoveDirection", 0.5f);
    }


    public void FixedUpdate()
    {
        if(isDead)
            return;

        Move();
    }



    void SetMoveDirection()     //몬스터 움직임 방향을 3초마다 설정
    {
        nextMove = Random.Range(-1,2);      //-1~1까지 

        int time = Random.Range(1,5);
        Invoke("SetMoveDirection", time);       //다음 방향 설정
    }


    void Move()     //몬스터 움직임 함수
    {   

        //몬스터의 앞 방향 벡터 (낭떨어지 체크)
        Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.3f, rb.position.y);
        Debug.DrawRay(frontVec, Vector2.down, new Color(0, 1, 0));

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



    //------플레이어 추적 이동을 트리거로 구현-------
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
            StartCoroutine("GoBack");   //1초 후 반대 방향으로 감
            Invoke("SetMoveDirection",3f);      //3초후 랜덤 방향 다시 설정
        }
    }



    //-------------------------------------
    IEnumerator GoBack()    //반대로 가는 함수
    {
        yield return new WaitForSeconds(1.0f);
        nextMove *= -1;
    }


    public void TakeDamage(Transform attacker, int damage)
    {
        am.SetBool("Grab", false);

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

    
    
    //-------히트박스 나타내기-------(인게임에서는 안보임) (히트박스 크기 조절하기 위해 만든 함수)
    private void OnDrawGizmos()     
    {                               
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hitBox.position,hitBox.localScale);
    }


}
