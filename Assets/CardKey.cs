using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardKey : MonoBehaviour
{
    public Transform enemy;
    public bool isDead;
    public bool isCardKey = false;
    public bool isSetActive = false;
    public Vector3 pos;
    void Start()
    {
        isDead = GameObject.Find("ArmMachine (3)").GetComponent<ArmMachine>().isDead;
    }
    void Update()
    {
        if (isDead)
        {
            gameObject.transform.position = enemy.transform.position;
            gameObject.SetActive(true);
            isSetActive = true;
            isDead = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (isSetActive == true)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                isCardKey = true;
                gameObject.SetActive(false);
            }
        }
    }
}
