using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableEnding : ConversationObject
{
    void Awake()
    {
        speaker = "비밀문서";
        content = new string[1];
        content[0] = "끝";
    
        eventIndex = (int)ConversationObject.objectEvent.ending;

    }

}
