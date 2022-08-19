using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : ConversationObject
{
    void Awake()
    {
        speaker = "비밀문서";
        content = new string[1];
        content[0] = "~~~~~";
    
        eventIndex = (int)ConversationObject.objectEvent.table;
    }
}
