using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
{
    public GameObject player;
    public Animator am;
    public bool isElevator = false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G) && isElevator == true)
        {
            StartCoroutine("LoadScene");
            isElevator = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        isElevator = true;
    }

    private IEnumerator LoadScene()
    {
        player.SetActive(false);
        yield return new WaitForSeconds (1f);
        
        am.Play("ElevatorUp", 0, 0);
        yield return new WaitForSeconds (2f);
            
        if (this.tag.Equals("Stage1"))
        {
            SceneManager.LoadScene("Stage2");
        }
        if (this.tag.Equals("Stage2"))
        {
            SceneManager.LoadScene("Stage3");
        }
    }
}
