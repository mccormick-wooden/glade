using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FairyEnemy : BaseEnemy
{

    [SerializeField]
    GameObject aoeAttackPrefab;

    Transform target;

    protected override void Start()
    {
        base.Start();

        isAttacking = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void UpdateAnimations()
    {
        // first thing:  if we're attacking, don't change and if we're dead, stay dead!
        if (damageable.IsDead)
            return;

        if (IsInAttackAnimation())
            return;

        Vector3 velocity = agent.velocity;
        float speed = agent.velocity.magnitude;

        float speedX = velocity.x;
        float speedY = velocity.y;
        float speedZ = velocity.z;

        float absSpeedX = Mathf.Abs(speedX);
        float absSpeedY = Mathf.Abs(speedY);
        float absSpeedZ = Mathf.Abs(speedZ);

        // don't worry about Y right now, don't need jumping just yet
        bool walkForward = (speedZ > 0.1 && (absSpeedZ > absSpeedX));
        bool walkBackward = (speedZ < -0.1 && (absSpeedZ > absSpeedX));

        bool walkLeft = (speedX < -0.1 && (absSpeedX > absSpeedZ));
        bool walkRight = (speedX > 0.1 && (absSpeedX > absSpeedZ));

        animator.SetBool(WALK_FORWARD, false);
        animator.SetBool(WALK_BACKWARD, false);
        animator.SetBool(STRAFE_LEFT, false);
        animator.SetBool(STRAFE_RIGHT, false);

        if (isAttacking)
        {
            animator.SetTrigger(CAST_SPELL);
            target = Player.transform;
            isAttacking = false;
        }
        else if (walkForward)
        {
            animator.SetBool(WALK_FORWARD, true);
        }
        else if (walkBackward)
        {
            animator.SetBool(WALK_BACKWARD, true);
        }
        else if (walkLeft)
        {
            animator.SetBool(STRAFE_LEFT, true);
        }
        else if (walkRight)
        {
            animator.SetBool(STRAFE_RIGHT, true);
        }

    }

    protected virtual void CastAOE()
    {
        Debug.Log("Cast AOE!");

        GameObject aoe = Instantiate(aoeAttackPrefab);
        aoe.transform.position = target.GetComponent<Collider>().bounds.center;
    }
}



/*
public class FairyEnemy : BaseEnemy
{


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        isAttacking = false;

        attackDelayTimeSeconds = 10.0f;

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
            DoAOEAttack();
        }
    }

    void DoAOEAttack()
    {
        //GameObject aoeAttack = Instantiate(aoeAttackPrefab, Player.transform.position, Player.transform.rotation, transform);
        GameObject aoeAttack = Instantiate(aoeAttackPrefab);
        aoeAttack.transform.position = Player.transform.position;
        isAttacking = false;
    }

    /*
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
    */
//}

