using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeElevator : ConversationObject
{

    void Awake()
    {
        speaker = "Player";
        content = new string[1];
        content[0] = "오늘도 빨리 끝내야겠군...";
        eventIndex = (int)ConversationObject.objectEvent.elevator;

    }

 
}
