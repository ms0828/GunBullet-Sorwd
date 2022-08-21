using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMonsterConversation : ConversationObject
{
    void Awake()
    {
        speaker = "Monster";
        content = new string[6];
        content[0] = "크크큭...";
        content[1] = "네 동료들과 가족들이 이렇게 된 것이 억울한가?";
        content[2] = "그렇다면 물어보지";
        content[3] = "누가 이런 상황을 만들었다고 생각하는가?";
        content[4] = "이 전쟁이 꼭 일어났어야만 했는가?";
        content[5] = "다- 네 탓이란 말이다!! 크하하하!!";
    } 

   
}
