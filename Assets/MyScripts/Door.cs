using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public GameObject gKey;
    public Animator am;


    public bool isTriggerEnter = false;

    void Start() 
    {
        am = GetComponent<Animator>();
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.G) && isTriggerEnter == true) // G키를 누르면
        {
            am.Play("DoorOpen", 0, 0); // 문이 열림
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // 플레이어가 가까이 오면
    { 
        if(other.gameObject.name.Equals("Player")) 
        {
            if (this.gameObject.tag.Equals("Stage1")) // 스테이지 1에서는
            {
                gKey.SetActive(true); // G키 이미지를 띄움
            }

            isTriggerEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) // 플레이어가 멀어지면
    {
        if(other.gameObject.name.Equals("Player")) 
        {
            if (this.gameObject.tag.Equals("Stage1"))
            {
                gKey.SetActive(false); // G키 이미지 삭제
            }

            isTriggerEnter = false;

            am.Play("DoorClose", 0, 0); // 문이 닫힘
        }
    }
}
