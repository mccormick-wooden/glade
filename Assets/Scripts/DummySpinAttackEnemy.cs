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
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isAttacking)
        {
            SpinAttackStep();
        }
        else
        {

        }
    }

    void SpinAttackStep()
    {
        // this enemy just spins to do an attack
        // dumb simple example
        if (isAttacking && degreesSpun > degreesPerAttack)
        {
            Debug.Log("Spun " + degreesSpun + ", attack done!");
            isAttacking = false;
            degreesSpun = 0.0f;
        }
        else
        {
            Debug.Log("Spinning: " + degreesSpun);
            float amountToSpin = degreesPerSecond * Time.deltaTime;
            degreesSpun += amountToSpin;
            transform.Rotate(0, amountToSpin, 0);
        }
    }

}
