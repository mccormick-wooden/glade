using Assets.Scripts.Abstract;
using UnityEngine;

public class Enemy : BaseDamageable
{
    private Animator animator;
    private Rigidbody rigidBody;
    public Transform Player;

    [SerializeField]
    private float minDistanceFromPlayer = 0F;

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

        if (Vector3.Magnitude(Player.transform.position - transform.position) > minDistanceFromPlayer)
        {
            transform.position += transform.forward * 5 * Time.deltaTime;
        }
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
