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
        content[1] = "기업에 잠입하여 비밀문서를 빼 올 것";
        content[2] = "cctv, 함정과 같은 장애물과 무수히 많은 적들이 도사리고 있으므로 각별히 주의할 것.";
    }
}
