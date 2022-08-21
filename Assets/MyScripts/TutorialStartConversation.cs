using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStartConversation : ConversationObject
{
   
    void Awake()
    {
        content = new string[1];
        speaker = "Player";
        content[0] = ".....";
    }


}
