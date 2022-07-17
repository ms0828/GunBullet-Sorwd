using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;


[System.Serializable]
public class UserData   //유저 데이터
{
    public int stage;

    public UserData(int _stage)
    {
        stage = _stage;
    }
}




public class GameManager : MonoBehaviour
{

    public static GameManager instance;     //솔루틴

    //-----유저 데이터-----
    public UserData userData;
    public int currentStage;
    
    //타이틀 버튼
    public Button loadGameButton;



    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }


    void Start()
    {
        //세이브 파일이 없다면, 불러오기 비활성화
        if(!File.Exists(Application.dataPath + "/DataBase/UserData.txt"))
        {
            loadGameButton.interactable = false;
        }



    }



    public void CreateUserData()
    {
        userData = new UserData(1);
        currentStage = 1;
        
        //데이터 만든 후, 저장
        string jdata = JsonConvert.SerializeObject(userData);
        File.WriteAllText(Application.dataPath + "/DataBase/UserData.txt",jdata);
    }

    public void LoadUserData()
    {
        string jdata = File.ReadAllText(Application.dataPath + "/DataBase/UserData.txt");
        userData = JsonConvert.DeserializeObject<UserData>(jdata);
        currentStage = userData.stage;
    }

    public void SaveUserData()
    {
        userData.stage = currentStage;
        string jdata = JsonConvert.SerializeObject(userData);
        File.WriteAllText(Application.dataPath + "/DataBase/UserData.txt",jdata);
    }


    public void NewGameButton()
    {
        CreateUserData();
        SceneManager.LoadScene(currentStage);
    }
    
    public void LoadGameButton()
    {
        LoadUserData();
        SceneManager.LoadScene(currentStage);
    }
    
    public void ExitButton()
    {
        Application.Quit();
    } 

}
