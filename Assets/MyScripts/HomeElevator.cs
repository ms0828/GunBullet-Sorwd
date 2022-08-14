using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeElevator : MonoBehaviour, IInterfaceObject
{
    public PlayerHomeManager pm;

    void Awake()
    {
        if(pm == null)
        {
            pm = GameObject.Find("PlayerHomeManager").GetComponent<PlayerHomeManager>();
        }
    }   
    public void Interface()
    {
        pm.StartHomeTimeLine();
    }
}
