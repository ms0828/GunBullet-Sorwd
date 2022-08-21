using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Basement : MonoBehaviour
{
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
            StartCoroutine (LoadScene());
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // 플레이어가 가까이 오면
    { 
        if(other.gameObject.name.Equals("Player")) 
        {
            isTriggerEnter = true;
        }
    }

    private IEnumerator LoadScene()
    {
        Debug.Log("1");

        am.enabled = true;
        am.Play("DoorOpen", 0, 0);

        yield return new WaitForSeconds (1f);

        if (this.gameObject.tag.Equals("Stage2"))
        {
            SceneManager.LoadScene("Basement");
        }

        if (this.gameObject.tag.Equals("Basement"))
        {
            SceneManager.LoadScene("Stage2");
        }
    }
}
