using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyPicture : ConversationObject
{
    void Awake()
    {
        content = new string[2];
    }

    void Start()
    {
        eventName = "Tutorial1Clear";
        speaker = "Player";
        content[0] = "가족 사진";
        content[1] = "......";
    }
}
