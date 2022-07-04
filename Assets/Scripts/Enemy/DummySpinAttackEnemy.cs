using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpinAttackEnemy : BaseEnemy
{
    private float degreesSpun;
    private float degreesPerAttack;
    private float degreesPerSecond;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        isAttacking = false;

        attackDelayTimeSeconds = 2.0f;

        degreesSpun = 0;
        degreesPerAttack = 360;
        degreesPerSecond = degreesPerAttack;

        desireToAttackPlayer = 4f;  
        desireToDefendBeacon = 2f; 
        desireToRunAndHeal = 1f;  
        desireToHealOthers = 0f;

        autoAttackPlayerDistanceToBeacon = 5f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (isAttacking)
        {
            SpinAttackStep();
        }
    }

    void SpinAttackStep()
    {
        // this enemy just spins to do an attack
        // dumb simple example
        if (isAttacking && degreesSpun > degreesPerAttack)
        {
            isAttacking = false;
            degreesSpun = 0.0f;
        }
        else
        {
            float amountToSpin = degreesPerSecond * Time.deltaTime;
            degreesSpun += amountToSpin;
            transform.Rotate(0, amountToSpin, 0);
        }
    }

}
