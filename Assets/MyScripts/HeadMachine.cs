using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMachine : Enemy
{

    new void Start()
    {
        base.Start();
        
        maxHp = 120;
        currentHp = 120;
        shortAttackPower = 20;
        longAttackPower = 15;
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
