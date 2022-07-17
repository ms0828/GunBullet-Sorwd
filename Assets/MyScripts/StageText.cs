using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageText : MonoBehaviour
{
    public Text stageText;

    void Start()
    {
        stageText.text = "Stage : " + (GameManager.instance.currentStage - 1).ToString();
    }

   
}
