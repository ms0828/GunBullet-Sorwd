using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class PlayerHomeManager : MonoBehaviour
{
    [SerializeField]
    private HomePlayer player;

    public PlayerUICanvas playerUi;

    
    private ConversationObject startConversation;

    public PlayableDirector chapterTimeline;


    //------타임라인 관련------
    [SerializeField]
    public PlayableDirector homeTimeline;

    void Awake()
    {
        if(playerUi == null)
        {
            playerUi = GameObject.Find("PlayerUICanvas").GetComponent<PlayerUICanvas>();
        }
        if(player == null)
        {
            player = GameObject.Find("HomePlayer").GetComponent<HomePlayer>();
        }
        if(homeTimeline == null)
        {
            Debug.Log("타임라인이 할당되지 않음(에러)");
        }

    }

    void Start()
    {
        chapterTimeline.gameObject.SetActive(true);
        chapterTimeline.Play();
    }



    public void StartHomeTimeLine()
    {
        homeTimeline.gameObject.SetActive(true);
        homeTimeline.Play();
    }

    public void EndHomeTimeLine()
    {
        homeTimeline.Stop();
        homeTimeline.gameObject.SetActive(false);
        
        GameManager.instance.SaveUserData(3);
        SceneManager.LoadScene("Stage1");
    }


    public void EndHomeChapterTimeLine()
    {
        chapterTimeline.gameObject.SetActive(false);
        
    }


}
