using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour    //적 추상클래스 (적은 이 클래스를 상속받아 추상 함수 구현해야함)
{
    //-----적 스텟-----
    public int maxHp;
    public int currentHp;
    public float speed;

    //-----기본 컴포넌트-----
    public Animator am;            // 에니메이터
    public Rigidbody2D rb;         // Rigidbody
    public SpriteRenderer sr;      //스프라이트 렌더러 (x축 filp 설정 위해)
    

    //-----상태 관련 bool-----
    public bool isDead = false;
    

    //-----움직임 방향 관련----
    public int nextMove = 0;       //0 정지, -1 왼쪽, 1 오른쪽
    public Vector3 leftDirection = new Vector3(-3,3,1);     //왼쪽으로 갈 때 몬스터에게 크기 할당(이미지 좌우 반전)
    public Vector3 rightDirection = new Vector3(3,3,1);    //오른쪽으로 갈 때


    //-----플레이어 캐싱-----
    public Player player;
    public int playerLayer;

    public void Awake()     //기본 컴포넌트 가져오기
    {
        sr = gameObject.GetComponent<SpriteRenderer>(); //스프라이트 렌더러 가져오기
        rb = gameObject.GetComponent<Rigidbody2D>();   //Rigidbody 가져오기
        am = gameObject.GetComponent<Animator>();       //에니메이터 가져오기

        player = GameObject.FindObjectOfType<Player>();
        playerLayer = 1 << LayerMask.NameToLayer("Player");
    }
}
