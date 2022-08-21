using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{    
    public GameObject box;
    public GameObject gKey;
    public Player player;
    public int life = 30;

    public bool isTriggerEnter = false;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.G) && isTriggerEnter == true) // G키를 누르면
        {
            Destroy(box); // 박스가 사라짐
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // 플레이어가 가까이 오면
    {
        if(other.gameObject.name.Equals("Player")) 
        {
            if (this.gameObject.tag.Equals("Stage1"))
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
            gKey.SetActive(false); // G키 이미지 삭제      

            isTriggerEnter = false;
        }
    }
}
