using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpZone : MonoBehaviour
{
    public float jumpZoneForce = 2f;
    bool isShortJumpZone = false;
    bool isLongJumpZone = false;

    Rigidbody2D rb;

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update() 
    {
        Jump();
    }

    void OnCollisionEnter2D(Collision2D col) 
    {        
        if (col.gameObject.tag == "ShortJumpZone")
        {
            isShortJumpZone = true;
        }

        if (col.gameObject.tag == "LongJumpZone")
        {
            isLongJumpZone = true;
        }
    }

    void Jump()
    {
        if (isShortJumpZone)
        {
            rb.AddForce(new Vector2(0, jumpZoneForce) * 100);
            isShortJumpZone = false;
        }

        if (isLongJumpZone)
        {
            rb.AddForce(new Vector2(0, jumpZoneForce) * 135);
            isLongJumpZone = false;
        }
    }
}
