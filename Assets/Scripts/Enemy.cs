using Assets.Scripts.Abstract;
using UnityEngine;

public class Enemy : BaseDamageable
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

    private void UpdateAnimations()
    {
    }

    private void ApplyTransforms()
    {
        transform.LookAt(Player);

        transform.position += transform.forward * 5 * Time.deltaTime;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        ApplyTransforms();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ShouldHandleCollisionAsAttack(other))
            HandleAttack(other.GetComponent<BaseWeapon>());    
    }

    protected override void HandleAttack(BaseWeapon attackingWeapon)
    {
        // Some custom behavior...
        base.HandleAttack(attackingWeapon); // then optionally, the base behavior
    }

    protected override void Die()
    {
        // Some custom behavior...
        base.Die(); // then optionally, the base behavior
    }
}
