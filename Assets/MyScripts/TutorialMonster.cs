using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMonster : Enemy, ITakeDamage
{
    [SerializeField]
    private TutorialManager tm;
    TutorialPlayer tutorialPlayer;

    new void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>(); //스프라이트 렌더러 가져오기
        rb = gameObject.GetComponent<Rigidbody2D>();   //Rigidbody 가져오기
        am = gameObject.GetComponent<Animator>();       //에니메이터 가져오기

        tutorialPlayer = GameObject.FindObjectOfType<TutorialPlayer>();
        playerLayer = 1 << LayerMask.NameToLayer("Player");
    
    }

    void Start()
    {
        maxHp = 150;
        currentHp = 150;
        speed = 1f;
    }

   
    void FixedUpdate()
    {
        if(isDead)
            return;
    }




    public void TakeDamage(Transform attacker, int damage)
    {  

        if(isDead == true)
            return;

        /* ----사운드----
        if(Random.Range(0,2) == 0)
            SoundManager.instance.EnemySfxSound(behaviorAudio,"ArmMachineHit1");
        else
            SoundManager.instance.EnemySfxSound(behaviorAudio,"ArmMachineHit2");
        */


        if(tm.currentStage == TutorialManager.tutorialStage.downAttack && tutorialPlayer.clearCheck == false)
        {
            if(tutorialPlayer.isDownAttack == true)
            {
                tm.ClearCurrentTutorial(tm.currentStage);
                tutorialPlayer.clearCheck = true;
            }
            
        }

        if(tm.currentStage == TutorialManager.tutorialStage.upAttack && tutorialPlayer.clearCheck == false)
        {
            if(tutorialPlayer.isUpAttack == true)
            {
                tm.ClearCurrentTutorial(tm.currentStage);
                tutorialPlayer.clearCheck = true;
            }
        }

        if(tm.currentStage == TutorialManager.tutorialStage.thrust && tutorialPlayer.clearCheck == false)
        {
            if(tutorialPlayer.isThrust == true)
            {
                tm.ClearCurrentTutorial(tm.currentStage);
                tutorialPlayer.clearCheck = true;
            }
        }

        if(tm.currentStage == TutorialManager.tutorialStage.shoot && tutorialPlayer.clearCheck == false)
        {
            if(tutorialPlayer.isShoot == true)
            {
                tm.ClearCurrentTutorial(tm.currentStage);
                tutorialPlayer.clearCheck = true;  //한 번만 실행되도록 만든 임시 bool 변수

                currentHp = 0;
                isDead = true;
                //am.SetTrigger("Dead");
                Destroy(gameObject, 2f);        //2초 후 사라짐
                return;
            }
        }


        am.SetTrigger("Hit");

        //넉백
        if(transform.position.x - attacker.position.x > 0)  //대상이 왼쪽에서 공격했다면
        {
            rb.AddForce(new Vector2(10f,0f), ForceMode2D.Impulse);       //(impulse => 순간적으로 힘을 준다)
        }
        else      //대상이 오른쪽에서 공격했다면
        {
            rb.AddForce(new Vector2(-10f,0f), ForceMode2D.Impulse);       //(impulse => 순간적으로 힘을 준다)
        }

        
    }
}
