using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public Animator am;
    
    float num = 0;
    float timer = 0;

    public bool isTriggerEnter = false;
    public bool isOpen = false;

    void Start() 
    {
        am = GetComponent<Animator>();
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.G) && isTriggerEnter == true) // G키를 누르면
        {
            isOpen = true;
        }

        if (isOpen == true)
        {
            timer += Time.deltaTime;
            
            StartCoroutine(EnemyAppear());
        }
    }
    private void OnTriggerEnter2D(Collider2D other) // 플레이어가 가까이 오면
    { 
        if(other.gameObject.name.Equals("Player")) 
        {
            isTriggerEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) // 플레이어가 멀어지면
    {
        if(other.gameObject.name.Equals("Player")) 
        {
            isTriggerEnter = false;
        }
    }

    IEnumerator EnemyAppear()
    {  
        if (timer > 1)
        {
            am.Play("DoorOpen", 0, 0);
        }
        
        if (timer > 2)
        {
            Debug.Log("0");
            enemy1.SetActive(true);
        }

        if (timer > 3)
        {
            Debug.Log("1");
            enemy2.SetActive(true);
        }

        if (timer > 4)
        {
            Debug.Log("2");
            enemy3.SetActive(true);
        }

        if (timer > 5)
        {
            timer = 0;
            isOpen = false;
        }

        yield return null;
    }
}
