using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableEnding : ConversationObject
{

    void Awake()
    {
        speaker = "Player";
        content = new string[2];
        content[0] = "(비밀문서를 획득했다.)";
        content[1] = "보라색 불빛 아래 출구로 나가면 되겠군";
        
        eventIndex = (int)ConversationObject.objectEvent.ending;
    }


}
