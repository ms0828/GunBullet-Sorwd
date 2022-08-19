using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyPicture : ConversationObject
{
    void Awake()
    {
        content = new string[2];
        speaker = "Player";
        content[0] = "가족 사진";
        content[1] = "......";

        eventIndex = (int)ConversationObject.objectEvent.tutorialClear;
    }

    
}
