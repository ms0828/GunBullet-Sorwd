using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHomeStartConversation : ConversationObject
{
    void Awake()
    {
        content = new string[2];
        speaker = "Player";
        content[0] = "또 이 꿈인가";
        content[1] = ".....";
    }
}
