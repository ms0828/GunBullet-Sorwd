using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IndicationKey : MonoBehaviour
{
    
    public GameObject player;
    public Animator am;

    void Awake()
    {
        am = GetComponent<Animator>();

        if(player == null)
        {
            if(SceneManager.GetActiveScene().name.Equals("TutorialScene"))
            {
                player = GameObject.Find("TutorialPlayer");
            }
            else if(SceneManager.GetActiveScene().name.Equals("PlayerHome"))
            {
                player = GameObject.Find("HomePlayer");
            }
            else
            {
                player = GameObject.Find("Player");
            }
            
        }
    }

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1f, player.transform.position.z);

        if(player.transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(1,1,1);
        }
    }


    public void SetKeyInterface()
    {
        am.SetTrigger("G");
    }
}
