using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Basement : MonoBehaviour
{
    public GameObject box;
    public Animator am;
    
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
            
            StartCoroutine(BoxAppear());
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

    IEnumerator BoxAppear()
    {  
        if (timer < 1)
        {
            am.Play("DoorOpen", 0, 0);
        }
        
        if (timer > 2)
        {
            box.SetActive(true);
        }
        if (timer > 3)
        {
            timer = 0;
            isOpen = false;
        }

        yield return null;
    }
}
