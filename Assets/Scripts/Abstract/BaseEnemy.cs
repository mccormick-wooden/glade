using Assets.Scripts.Abstract;
using UnityEngine;

public class BaseEnemy : BaseDamageable
{
    private Animator animator;
    private Rigidbody rigidBody;
    public GameObject playerGameObject;
    public Transform Player;

    // Height should be filled in by a specific subclass 
    public float Height; 

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        Player = GameObject.Find("Player").transform;
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
