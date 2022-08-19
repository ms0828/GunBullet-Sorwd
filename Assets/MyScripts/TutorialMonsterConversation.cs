using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMonsterConversation : ConversationObject
{
    void Awake()
    {
        speaker = "Monster";
        content = new string[3];
        content[0] = "test1";
        content[1] = "test2";
        content[2] = "test3";
    } 

   
}
