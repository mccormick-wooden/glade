using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomScript : BaseEnemy
{
    [SerializeField]
    GameObject aboutToBlowParticleSystemPrefab;

    [SerializeField]
    GameObject explosionPrefab;


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

        animator.SetBool(RUN_FORWARD, false);
        animator.SetBool(RUN_BACKWARD, false);
        animator.SetBool(STRAFE_LEFT, false);
        animator.SetBool(STRAFE_RIGHT, false);

        if (isAttacking)
        {
            animator.SetTrigger(CAST_SPELL);

            

            isAttacking = false;
        }
        else if (walkForward)
        {
            animator.SetBool(RUN_FORWARD, true);
        }
        else if (walkBackward)
        {
            animator.SetBool(RUN_BACKWARD, true);
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

    protected virtual void Explode()
    {
        Debug.Log("Explode!!");

        GameObject aboutToBlow = Instantiate(explosionPrefab, transform);
        aboutToBlow.transform.position = transform.GetComponent<Collider>().bounds.center;

        damageable.InstantKill();
    }
}
