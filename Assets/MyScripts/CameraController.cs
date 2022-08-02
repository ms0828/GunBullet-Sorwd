using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 cameraPosition = new Vector3(0, 3, -5);
    Transform playerTransform;
    private void Start() 
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }
    void FixedUpdate()
    {
    	transform.position = playerTransform.position + cameraPosition;
    }
}
