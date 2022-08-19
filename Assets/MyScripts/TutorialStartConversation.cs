using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStartConversation : ConversationObject
{
   
    void Awake()
    {
        content = new string[2];
        speaker = "Player";
        content[0] = "여긴어디(test)";
        content[1] = "나는누구(test)";
    }


}
