using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour, IInterfaceObject
{
    public GameObject player;
    public Animator am;


    void Awake()
    {
        player = GameObject.Find("Player");
    }


    private IEnumerator LoadScene()
    {
        player.SetActive(false);
        yield return new WaitForSeconds (1f);
        
        am.Play("ElevatorUp", 0, 0);
        yield return new WaitForSeconds (2f);
            
        if (SceneManager.GetActiveScene().name.Equals("Stage1"))
        {
            GameManager.instance.SaveUserData(4);
            SceneManager.LoadScene("Stage2");
        }
        if (SceneManager.GetActiveScene().name.Equals("Stage2"))
        {
            GameManager.instance.SaveUserData(5);
            SceneManager.LoadScene("Stage3");
        }
    }

    public void Interface()
    {
        if (SceneManager.GetActiveScene().name.Equals("Stage1") && CardKey.isCardKey == true)
        {
            StartCoroutine("LoadScene");
        }
        else if(SceneManager.GetActiveScene().name.Equals("Stage2") && Computer.noLimit == true)
        {
            StartCoroutine("LoadScene");
        }
    }
}
