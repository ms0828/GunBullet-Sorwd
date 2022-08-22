using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senser : MonoBehaviour
{
    public bool isPlayer;

    private void OnTriggerEnter2D(Collider2D other) // 범위 안에 플레이어가 감지되면
    {
        if (other.gameObject.name.Equals("Player"))
        {
            isPlayer = true;

            other.GetComponent<ITakeDamage>().TakeDamage(this.transform, 200);
        }
    }
}
