using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StageText : MonoBehaviour
{
    public Text stageText;

    void Start()
    {   
        if(SceneManager.GetActiveScene().name.Equals("TutorialScene"))
        {
            stageText.text = "Tutorial";
        }
        else if(SceneManager.GetActiveScene().name.Equals("PlayerHome"))
        {
            stageText.text = "PlayerHome";
        }
        else if(SceneManager.GetActiveScene().name.Equals("Stage1"))
        {
            stageText.text = "Stage1";
        }
        else if(SceneManager.GetActiveScene().name.Equals("Stage2"))
        {
            stageText.text = "Stage2";
        }
        else if(SceneManager.GetActiveScene().name.Equals("Stage3"))
        {
            stageText.text = "Stage3";
        }
        
    }

   
}
