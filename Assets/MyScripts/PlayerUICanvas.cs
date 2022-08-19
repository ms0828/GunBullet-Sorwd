using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class PlayerUICanvas : MonoBehaviour
{

    public GameObject[] hpBar;
    public GameObject[] bulletCount;
    

    //-------대화 박스 관련--------

    public ConversationObject conversationInfo;
    public GameObject dialogBox;
    public Text speakerText;
    public Text contentText;

    public string speaker;
    public string[] content;

    public int maxPage = 0;
    public int curPage = 0;

    public bool isPrintingDialog = false;

    Coroutine dialogCoroutine;
    WaitForSeconds dialogTempo = new WaitForSeconds(0.1f);

    

    //--------타임라인---------
    public GameObject timeline;    //매 스테이지 마다 있는 타임라인 부모 오브젝트 (비활성화된 타임라인 find 위해 사용)
    public PlayableDirector playTimeline;      //진행할 타임라인 담아서 사용

    void Awake()
    {
        if(dialogBox == null)
        {
            dialogBox = transform.Find("DialogBox").gameObject;
        }
        if(speakerText == null)
        {
            speakerText = dialogBox.transform.Find("SpeakerText").GetComponent<Text>();
        }
        if(contentText == null)
        {
            contentText = dialogBox.transform.Find("ContentText").GetComponent<Text>();
        }

        timeline = GameObject.Find("Timelines");

    }

    public void SetPlayerHpBar(int currentHp)
    {
        int currentHpBarIndex = (int)(currentHp * 0.1);
 
        for(int i = 0; i < 20; i++)
        {   
            if(i<currentHpBarIndex)
                hpBar[i].SetActive(true);
            else
                hpBar[i].SetActive(false);
        }
    }


    public void SetPlayerBulletCount(int bulletCnt)
    {
        for(int i=0; i < 3; i++)
        {
            if(i < bulletCnt)
                bulletCount[i].SetActive(true);
            else
                bulletCount[i].SetActive(false);
        }
    }


    public void StartDialog(ConversationObject _conversationInfo)
    {
        conversationInfo = _conversationInfo;
        dialogBox.SetActive(true);
        speaker = conversationInfo.speaker;
        content = conversationInfo.content;

        maxPage = content.Length - 1;
        curPage = 0;

        speakerText.text = speaker;

        dialogCoroutine = StartCoroutine(PrintDialogPage());
    }

    public void NextDialog()
    {
        if(isPrintingDialog == true)    //대화가 아직 찍히고 있으면 
        {
            contentText.text = "";
            isPrintingDialog = false;

            if(dialogCoroutine != null)     //대화 찍히는 애니메이션 생략
            {
                StopCoroutine(dialogCoroutine);
                contentText.text = content[curPage];
            }
        }

        if(curPage < maxPage)
        {
            curPage++;
            dialogCoroutine = StartCoroutine(PrintDialogPage());
        }
        else        //다음 대화가 없으면 종료
        {  
            ExitDialog();
            return;
        }

    }

    public void ExitDialog()
    {
        speakerText.text = "";
        contentText.text = "";
        maxPage = 0;
        curPage = 0;

        speaker = null;
        content = null;

        dialogBox.SetActive(false);


        //-------특정 대화 종료 이벤트--------
        if(conversationInfo.eventIndex == (int)ConversationObject.objectEvent.tutorialClear)
        {   
            SceneManager.LoadScene("PlayerHome");
        }
        else if(conversationInfo.eventIndex == (int)ConversationObject.objectEvent.table)
        {
            playTimeline = null;
            playTimeline = timeline.transform.Find("TableTimeline").GetComponent<PlayableDirector>();
            playTimeline.gameObject.SetActive(true);
            playTimeline.Play();
        }


        conversationInfo = null;
    }

    public IEnumerator PrintDialogPage()
    {
        isPrintingDialog = true;
        contentText.text = "";

        for(int i=0; i<content[curPage].Length; i++)
        {   
            contentText.text += content[curPage][i];
                
            yield return dialogTempo;
        }

        isPrintingDialog = false;
    }


    public void ExitTimeline()
    {
        playTimeline.gameObject.SetActive(false);
    }


}
