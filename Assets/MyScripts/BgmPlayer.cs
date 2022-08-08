using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmPlayer : MonoBehaviour
{
    void Start()
    {
        SoundManager.instance.BgmSound();
    }
}
