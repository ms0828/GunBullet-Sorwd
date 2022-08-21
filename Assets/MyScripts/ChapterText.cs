using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChapterText : MonoBehaviour
{
    public Text chapterText;
    public string content;

    WaitForSeconds dialogTempo = new WaitForSeconds(0.1f);

    void Awake()
    {
        if(chapterText == null)
        {
            chapterText = GetComponent<Text>();
        }


        if(SceneManager.GetActiveScene().name.Equals("TutorialScene"))
        {
            content = "Chapter 1.폐허가 된 도시";
        }
        else if(SceneManager.GetActiveScene().name.Equals("PlayerHome"))
        {
            content = "Chapter 2.잠입";
        }
        else if(SceneManager.GetActiveScene().name.Equals("Stage3"))
        {
            
        }
    }


    void Start()
    {
        StartCoroutine(PrintChapterText());
    }


    public IEnumerator PrintChapterText()
    {
        chapterText.text = "";
        

        for(int i=0; i<content.Length; i++)
        {   
            chapterText.text += content[i];
            
            yield return dialogTempo;
        }
    }
}
