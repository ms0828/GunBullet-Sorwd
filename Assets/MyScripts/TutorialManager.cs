using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    public KeyTutorial keyTutorial;

    [SerializeField]
    private TutorialPlayer player;

    public PlayerUICanvas playerUi;


    public enum tutorialStage{left,right,jump,conversation,downAttack,upAttack,thrust,shoot};
    public tutorialStage currentStage;
    
    public bool[] clearStaus = new bool[8];


    WaitForSeconds clearTimeTempo = new WaitForSeconds(1.0f);

    public GameObject tutorialCanvas;
    public Text attackIndicationText;

    //----------몬스터 대화--------
    public ConversationObject monsterConversation;


    public PlayableDirector chapterTimeline;


    public GameObject interfaceWall;

    void Awake()
    {
        playerUi = GameObject.Find("PlayerUICanvas").GetComponent<PlayerUICanvas>();

        if(tutorialCanvas == null)
            tutorialCanvas = transform.Find("TutorialCavas").gameObject;
        if(attackIndicationText == null)
            attackIndicationText = tutorialCanvas.transform.Find("AttackIndicationText").GetComponent<Text>();


        monsterConversation = GameObject.Find("Monster").transform.Find("TutorialMonsterConversation").GetComponent<TutorialMonsterConversation>();
        
    }

    void Start()
    {
        currentStage = tutorialStage.left;
        
        chapterTimeline.gameObject.SetActive(true);
        chapterTimeline.Play();
    }


    public void SetKeyTutorial(tutorialStage _currentStage)   //현재 진행중인 튜토리얼 단계 인자로 받음
    {
        switch(_currentStage)
        {
            case tutorialStage.left:
            {
                keyTutorial.gameObject.SetActive(true);
                keyTutorial.am.SetTrigger("A");
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "A로 좌측 이동";
                break;
            }
            case tutorialStage.right:
            {
                keyTutorial.gameObject.SetActive(true);
                keyTutorial.am.SetTrigger("D");
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "D로 우측 이동";
                break;
            }
            case tutorialStage.jump:
            {
                keyTutorial.gameObject.SetActive(true);
                keyTutorial.am.SetTrigger("Space");
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "Space bar로 점프";
                break;
            }
            case tutorialStage.conversation:
            {
                keyTutorial.gameObject.SetActive(false);
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "G를 눌러 시체를 살펴보세요 (상호작용 키)";
                break;
            }
            case tutorialStage.downAttack:
            {
                keyTutorial.gameObject.SetActive(false);
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "왼쪽 클릭으로 하단 베기";
                break;
            }
            case tutorialStage.upAttack:
            {
                keyTutorial.gameObject.SetActive(false);
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "W + 왼쪽클릭으로 상단 베기";
                break;
            }
            case tutorialStage.thrust:
            {
                keyTutorial.gameObject.SetActive(false);
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "E + 왼쪽클릭으로 찌르기";
                break;
            }
            case tutorialStage.shoot:
            {
                keyTutorial.gameObject.SetActive(false);
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "우클릭으로 포격 발사";
                break;
            }
        }
    }


    public void ClearCurrentTutorial(tutorialStage _currentStage)
    {
        switch(_currentStage)
        {
            
            case tutorialStage.left:
            {
                clearStaus[(int)_currentStage] = true;
                currentStage = (tutorialStage)((int)_currentStage + 1);
                StartCoroutine(NextTutorial());
                break;
            }
            case tutorialStage.right:
            {
                clearStaus[(int)_currentStage] = true;
                currentStage = (tutorialStage)((int)_currentStage + 1);
                StartCoroutine(NextTutorial());
                break;
            }
            case tutorialStage.jump:
            {
                clearStaus[(int)_currentStage] = true;
                currentStage = (tutorialStage)((int)_currentStage + 1);
                StartCoroutine(NextTutorial());
                break;
            }
            case tutorialStage.conversation:
            {
                clearStaus[(int)_currentStage] = true;
                currentStage = (tutorialStage)((int)_currentStage + 1);
                interfaceWall.SetActive(false);
                attackIndicationText.gameObject.SetActive(false);
                
                break;
            }
            case tutorialStage.downAttack:
            {
                clearStaus[(int)_currentStage] = true;
                currentStage = (tutorialStage)((int)_currentStage + 1);
                StartCoroutine(NextTutorial());
                break;
            }
            case tutorialStage.upAttack:
            {
                clearStaus[(int)_currentStage] = true;
                currentStage = (tutorialStage)((int)_currentStage + 1);
                StartCoroutine(NextTutorial());
                break;
            }
            case tutorialStage.thrust:
            {
                clearStaus[(int)_currentStage] = true;
                currentStage = (tutorialStage)((int)_currentStage + 1);
                StartCoroutine(NextTutorial());
                break;
            }
            case tutorialStage.shoot:
            {
                clearStaus[(int)_currentStage] = true;
                currentStage = (tutorialStage)((int)_currentStage + 1);
                keyTutorial.gameObject.SetActive(false);
                attackIndicationText.gameObject.SetActive(false);
                break;
            }
            default:
                break;
        }
    }



    IEnumerator NextTutorial()
    {
        keyTutorial.gameObject.SetActive(false);

        yield return clearTimeTempo;

        player.clearCheck = false;      //check변수 초기화 (다음 튜토리얼로 넘어가기 위한)

        SetKeyTutorial(currentStage);
    }



    public void PrintMonsterDialog()
    {
        playerUi.StartDialog(monsterConversation);

        Invoke("DownAttackIndication", 3f);
    }

    public void DownAttackIndication()
    {
        player.clearCheck = false;
        SetKeyTutorial(currentStage);
    }


    public void EndChapterTimeline()
    {
        chapterTimeline.gameObject.SetActive(false);
        SetKeyTutorial(currentStage);
    }

}
