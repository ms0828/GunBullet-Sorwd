using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : ConversationObject
{
    void Awake()
    {
        content = new string[1];
        speaker = "시체";
        content[0] = ".....";
    }

    

}
