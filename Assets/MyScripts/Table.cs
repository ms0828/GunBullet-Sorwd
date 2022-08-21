using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : ConversationObject
{
    void Awake()
    {
        speaker = "Player";
        content = new string[1];
        content[0] = "비밀문서.. 여기있나";
    
        eventIndex = (int)ConversationObject.objectEvent.table;
    }

    void Start()
    {
        gameObject.GetComponent<TableEnding>().enabled = false;
    }
}
