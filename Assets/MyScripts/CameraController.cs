using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 targetPosition;

    //----영역 제한----
    public BoxCollider2D mapBoundary;
    float minX, maxX, minY, maxY;



    void Awake() 
    {
        if(SceneManager.GetActiveScene().name.Equals("TutorialScene"))
        {
            player = GameObject.Find("TutorialPlayer");
        }
        else if(SceneManager.GetActiveScene().name.Equals("PlayerHome"))
        {
            player = GameObject.Find("HomePlayer");
        }
        else
        {
            player = GameObject.Find("Player");
        }
        
        mapBoundary = GameObject.Find("MapBoundary").GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        //카메라의 영역을 설정
        SetLimits();
    }

    void Update()
    {
        if(player != null)      //카메라의 위치를 플레이어 위치로
        {
            targetPosition.Set(player.transform.position.x, player.transform.position.y,transform.position.z);
            transform.position = targetPosition;
        }
    }

    void LateUpdate() 
    {
        //Mathf.Clamp는 범위 안의 값을 반환하고 범위를 벗어나면 인자로 들어온 최소, 최대를 반환한다.
        float xClamp = Mathf.Clamp(targetPosition.x,minX,maxX);
        float yClamp = Mathf.Clamp(targetPosition.y,minY,maxY);

        //카메라의 위치 제한
        transform.position = new Vector3(xClamp, yClamp, -10f);
    }

    void SetLimits()
    {
        float height = 2f * Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;

        minX = mapBoundary.transform.position.x - mapBoundary.bounds.size.x * 0.5f + width / 2f;
        maxX = mapBoundary.transform.position.x + mapBoundary.bounds.size.x * 0.5f - width / 2f;

        minY = mapBoundary.transform.position.y - mapBoundary.bounds.size.y * 0.5f + height / 2f;
        maxY =  mapBoundary.transform.position.y + mapBoundary.bounds.size.y * 0.5f - height / 2f;

    }
}
