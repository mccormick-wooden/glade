using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRangedAttackEnemy : BaseEnemy
{
    [SerializeField]
    GameObject arrowPrefab;

    [SerializeField]
    float arrowForce;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        isAttacking = false;

        attackDelayTimeSeconds = 2.0f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isAttacking)
        {
            ShootArrow();
        }
        else
        {

        }
    }

    void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, transform);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.InUse = true;

        Vector3 centerOfPlayer = Player.transform.GetComponent<CapsuleCollider>().bounds.center;

        arrow.transform.position = transform.GetComponent<CapsuleCollider>().bounds.center;

        arrow.transform.LookAt(centerOfPlayer);

        Vector3 v = arrow.transform.position - centerOfPlayer;

        Debug.DrawRay(arrow.transform.position, -v * 50, Color.cyan, 1f);

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(arrowForce * -v, ForceMode.Impulse);
        isAttacking = false;
        
    }

    /*
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
    */
}
