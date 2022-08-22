using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Playables;


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


    //------타임라인------
    public PlayableDirector timeLine;


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


    //-----------타이틀 화면 관련--------------
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

    public void SaveUserData(int stage)
    {
        currentStage = stage;
        userData.stage = currentStage;
        string jdata = JsonConvert.SerializeObject(userData);
        File.WriteAllText(Application.dataPath + "/DataBase/UserData.txt",jdata);
    }



}
