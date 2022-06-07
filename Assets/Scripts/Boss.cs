using System;
using Assets.Scripts.Abstract;
using UnityEngine;

// Blatantly copied from Enemy.cs - did not want to step over changes in Enemy.cs for other scenes
public class Boss : BaseDamageable
{
    private Animator animator;
    private Rigidbody rigidBody;
    public Transform Player;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    protected virtual void UpdateAnimations() { }

    private void ApplyTransforms()
    {
        // TODO: Ideally transforms and triggered animations are handled separately
        var currentPositionIgnoringY = new Vector3(
            transform.position.x,
            Player.transform.position.y,
            transform.position.z
        );
        var testRotation = Quaternion.LookRotation(
            Player.transform.position - currentPositionIgnoringY
        );

        float angle = Quaternion.Angle(transform.rotation, testRotation);
        // Want to minimize how much rotating is happening when small changes are needed
        if (angle >= 10.0f)
        {
            var relativeRotation = Quaternion.LookRotation(
                Player.transform.position - transform.position
            );
            rigidBody.MoveRotation(
                Quaternion.Slerp(transform.rotation, relativeRotation, 1 * Time.deltaTime)
            );
            animator.SetBool("isTurning", true);
        }
        else
        {
            animator.SetBool("isTurning", false);
        }

        float distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);
        if (distanceToPlayer >= 16.0f)
        {
            rigidBody.MovePosition(transform.position + (transform.forward * (5 * Time.deltaTime)));
            animator.SetBool("isTurning", false);
            animator.SetBool("Walk Forward", true);
        }
        else
        {
            animator.SetBool("Walk Forward", false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (CurrentHp > 0)
            UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (CurrentHp > 0)
            ApplyTransforms();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ShouldHandleCollisionAsAttack(other))
            HandleAttack(other.GetComponent<BaseWeapon>());
    }

    protected override void HandleAttack(BaseWeapon attackingWeapon)
    {
        // TODO: Use booleans per professor?
        animator.SetTrigger("Take Damage");
        base.HandleAttack(attackingWeapon); // then optionally, the base behavior
    }

    protected override void Die()
    {
        animator.SetTrigger("Die");
    }
}
