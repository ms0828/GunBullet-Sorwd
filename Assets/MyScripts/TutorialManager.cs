using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private string[] monsterDialog = new string[3];

    void Awake()
    {
        playerUi = GameObject.Find("PlayerUICanvas").GetComponent<PlayerUICanvas>();

        if(tutorialCanvas == null)
            tutorialCanvas = transform.Find("TutorialCavas").gameObject;
        if(attackIndicationText == null)
            attackIndicationText = tutorialCanvas.transform.Find("AttackIndicationText").GetComponent<Text>();


        monsterDialog[0] = ".......";
        monsterDialog[1] = "sdaffsda";
        monsterDialog[2] = "qqqqqqqq";
    
    }

    void Start()
    {
        currentStage = tutorialStage.left;

        SetKeyTutorial(currentStage);
    }


    public void SetKeyTutorial(tutorialStage _currentStage)   //현재 진행중인 튜토리얼 단계 인자로 받음
    {
        switch(_currentStage)
        {
            case tutorialStage.left:
            {
                keyTutorial.gameObject.SetActive(true);
                keyTutorial.am.SetTrigger("A");
                
                break;
            }
            case tutorialStage.right:
            {
                keyTutorial.gameObject.SetActive(true);
                keyTutorial.am.SetTrigger("D");
                
                break;
            }
            case tutorialStage.jump:
            {
                keyTutorial.gameObject.SetActive(true);
                keyTutorial.am.SetTrigger("Space");
                break;
            }
            case tutorialStage.conversation:
            {
                keyTutorial.gameObject.SetActive(true);
                keyTutorial.am.SetTrigger("G");
                break;
            }
            case tutorialStage.downAttack:
            {
                keyTutorial.gameObject.SetActive(false);
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "L_Button";
                break;
            }
            case tutorialStage.upAttack:
            {
                keyTutorial.gameObject.SetActive(false);
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "W + L_Button";
                break;
            }
            case tutorialStage.thrust:
            {
                keyTutorial.gameObject.SetActive(false);
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "E + L_Button";
                break;
            }
            case tutorialStage.shoot:
            {
                keyTutorial.gameObject.SetActive(false);
                attackIndicationText.gameObject.SetActive(true);
                attackIndicationText.text = "";
                attackIndicationText.text = "R_Button";
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
                keyTutorial.gameObject.SetActive(false);

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

        if(currentStage != tutorialStage.conversation)
            SetKeyTutorial(currentStage);
    }



    public void PrintMonsterDialog()
    {
        playerUi.StartDialog("monster", monsterDialog);

        Invoke("DownAttackIndication", 3f);
    }

    public void DownAttackIndication()
    {
        player.clearCheck = false;
        attackIndicationText.gameObject.SetActive(true);
        attackIndicationText.text = "";
        attackIndicationText.text = "L_Button";
    }


    public IEnumerator ClearTutorial1()
    {
        yield return new WaitForSeconds(3.0f);

        SceneManager.LoadScene("PlayerHome");
    }
}
