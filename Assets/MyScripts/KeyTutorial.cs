using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTutorial : MonoBehaviour
{
    [SerializeField]
    private TutorialPlayer player;
    public Animator am;

    void Awake()
    {
        am = GetComponent<Animator>();
    }

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1.0f, player.transform.position.z);
    }



}
