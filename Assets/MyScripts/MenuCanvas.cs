using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

public class MenuCanvas : MonoBehaviour
{
    public Button loadGameButton;
  
    void Start()
    {
        //세이브 파일이 없다면, 불러오기 비활성화
        if(!File.Exists(Application.dataPath + "/UserData.txt"))
        {
            loadGameButton.interactable = false;
        }

    }


    public void NewGameButton()
    {
        
        if(!File.Exists(Application.dataPath + "/UserData.txt"))
            GameManager.instance.CreateUserData();
        

        GameManager.instance.currentStage = 1;
        SceneManager.LoadScene(GameManager.instance.currentStage);
        
    }
    
    public void LoadGameButton()
    {
        GameManager.instance.LoadUserData();
        SceneManager.LoadScene(GameManager.instance.currentStage);
    }
    
    public void ExitButton()
    {
        Application.Quit();
    } 
}
