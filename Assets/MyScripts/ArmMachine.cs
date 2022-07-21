using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmMachine : Enemy
{

    new void Start()
    {
        base.Start();   //Enemy의 Start 먼저 실행

        maxHp = 100;
        currentHp = 100;
        shortAttackPower = 15;
        longAttackPower = 20;
    }

    void Update()
    {
        
    }


    public override void ShortAttack()     //근접 공격
    {

    }

    public override void LongAttack()      //원거리 공격
    {

    }
}
