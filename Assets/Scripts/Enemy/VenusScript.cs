using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenusScript : BaseEnemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //weapon = base.gameObject.transform.Find("RigHead").Find("Bite")?.GetComponent<BiteAttack>();
        weapon = GetComponentInChildren<BiteAttack>();

        isAttacking = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    protected override void UpdateAnimations()
    {
        if (IsInAttackAnimation())
        {
            weapon.InUse = true;
            return;
        }

        weapon.InUse = false;

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
            animator.SetTrigger(MELEE_ATTACK);

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
}
