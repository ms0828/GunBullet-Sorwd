using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeMonitor : ConversationObject
{
    void Awake()
    {
        content = new string[3];
    }

    void Start()
    {
        speaker = "모니터";
        content[0] = "-다음 의뢰 내용-";
        content[1] = "A기업에 잠입하여 비밀 문서를 빼올 것.";
        content[2] = "무수히 많은 함정, cctv, 용병들을 주의할 것";
    }
}
