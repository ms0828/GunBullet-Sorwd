using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUICanvas : MonoBehaviour
{

    public GameObject[] hpBar;
    public GameObject[] bulletCount;
    

    //-------대화 박스 관련--------
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

    

    void Awake()
    {
        dialogBox = transform.Find("DialogBox").gameObject;
        speakerText = dialogBox.transform.Find("SpeakerText").GetComponent<Text>();
        contentText = dialogBox.transform.Find("ContentText").GetComponent<Text>();

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


    public void StartDialog(string _speaker, string[] _content)
    {
        dialogBox.SetActive(true);
        speaker = _speaker;
        content = _content;

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


}
