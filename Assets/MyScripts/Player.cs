using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, ITakeDamage
{
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



    //-----움직임 방향 관련----
    private Vector3 lScale = new Vector3(-3,3,1);
    private Vector3 rScale = new Vector3(3,3,1);
    private Vector2 leftDirection = new Vector2(-1,0);
    private Vector2 rightDirection = new Vector2(1,0);
    public int playerDirection = 1;     //오른쪽 보고 있을 때 1, 왼쪽 보고 있을 때 0
    

    //-----사격 관련-----
    public Transform muzzlePos;
    public GameObject bulletPrefeb;

        

    public int holdingGauge = 0;
   
    public void Awake()
    {

    }

    public void Start()
    {
        maxHp = 5000;
        currentHp = 5000;
        speed = 5.0f;
        jumpPower = 5.0f;
    }

    public void Update()
    {
        PlayerControll();
    }





    public void PlayerControll()
    {
        
        if(isHolding)       //홀딩 상태면 컨트롤 불가
        {
            if(Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.D))
            {
                holdingGauge += 10;
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
            if(!IsJump())       //현재 공중에 있지 않으면 (땅에 닿아있으면)
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);       //(impulse => 순간적으로 힘을 준다)
            }
        }



        //----------플레이어 공격 관련-----
        if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.W))
        {
            am.SetTrigger("UpAttack");
        }
        else if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.E))
        {
            am.SetTrigger("ThrustAttack");
        }
        else if(Input.GetMouseButtonDown(0))
        {
            am.SetTrigger("DownAttack");
        }
        else if(Input.GetMouseButtonDown(1))
        {
            am.SetTrigger("ShootAttack");
        }
       

    }

    void Shooting()     //포격 에니메이션에서 호출됨 (에니메이션 이벤트 함수)   -> 총알 발사
    {
        Bullet bullet = Instantiate(bulletPrefeb, muzzlePos.position, muzzlePos.rotation).GetComponent<Bullet>();
        if(playerDirection == 1)    //오른쪽 보고 있으면
        {
            bullet.SetBullet(rightDirection);
        }
        else    //왼쪽 보고 있으면
        {
            bullet.SetBullet(leftDirection);
        }
        
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
    }



    public void TakeDamage(Transform attacker, int damage)
    {
        isHolding = false;      //피격 시, 홀딩 해제
        am.SetBool("Holding",false);



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


    public IEnumerator Holding(HeadMachine attacker)   //HeadMachine이 잡기 공격을 했을 때 실행되는 함수
    {

        isHolding = true;
        am.SetBool("Holding",true);
        holdingGauge = 0;

        for(int i=0; i<20; i++)
        {
            if(holdingGauge >= 100)
            {
                am.SetBool("Holding",false);
                isHolding = false;
                attacker.TakeDamage(this.transform, 0);        //탈출 성공하면 적 밀쳐냄
                yield break;
            }

            holdingGauge -= 10;
            if(holdingGauge <= 0)
                holdingGauge = 0;


            yield return new WaitForSeconds(0.2f);
        }
        
    }


    //-------히트박스 나타내기-------(인게임에서는 안보임) (히트박스 크기 조절하기 위해 만든 함수)
    private void OnDrawGizmos()     
    {                               
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hitBox.position,hitBox.localScale);
    }

}
