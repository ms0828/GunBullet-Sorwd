using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour, IInterfaceObject
{
    public PlayableDirector endingTimeline;

    public void Interface()
    {
        endingTimeline.gameObject.SetActive(true);
        endingTimeline.Play();
    }


    public void Ending()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
