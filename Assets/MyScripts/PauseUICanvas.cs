using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUICanvas : MonoBehaviour
{
    //일시정지 캔버스
    public GameObject pauseCanvas;
    

    //----------게임 일시정지 버튼 관련-------------
    public void PauseButton()
    {
        Time.timeScale = 0;
        pauseCanvas.SetActive(true);
    }
    public void GoTitleButton()
    {
        Time.timeScale = 1;
        GameManager.instance.SaveUserData(GameManager.instance.currentStage);
        SceneManager.LoadScene("TitleScene");
    }

    public void ResumeButton()
    {
        Time.timeScale = 1;
        pauseCanvas.SetActive(false);
    }
}
