using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRangedAttackEnemy : BaseEnemy
{
    [SerializeField]
    private GameObject arrowPrefab;

    [SerializeField]
    private float arrowForce;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        isAttacking = false;

        attackDelayTimeSeconds = 2.0f;

        desireToAttackPlayer = 4f;
        desireToDefendBeacon = 1f;
        desireToRunAndHeal = 4f; // easy to scare off
        desireToHealOthers = 0f;

        autoAttackPlayerDistanceToBeacon = 3f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (isAttacking)
        {
            ShootArrow();
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
}
