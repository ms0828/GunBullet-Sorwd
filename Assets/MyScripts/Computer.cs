using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Computer : MonoBehaviour, IInterfaceObject
{
    public static bool noLimit = false;


    public void Interface()
    {
        noLimit = true;
    }
}
