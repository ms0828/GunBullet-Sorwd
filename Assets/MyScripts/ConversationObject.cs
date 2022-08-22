using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationObject : MonoBehaviour
{
    public int eventIndex = 0;
    public string speaker;
    public string[] content;

    public Sprite speakerImage;

    public enum objectEvent{tutorialClear = 1, table, ending, elevator};


}
