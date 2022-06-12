using UnityEngine;
using System;
using Assets.Scripts.Abstract;


public class BaseEnemy : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidBody;
    public GameObject playerGameObject;
    public Transform Player;

    // Height should be filled in by a specific subclass 
    [SerializeField]
    public float Height;
    
    [SerializeField]
    public float Speed;

    protected BaseWeapon weapon;

    [SerializeField]
    protected float minDistanceFromPlayer = 0F;
    [SerializeField]
    protected float maxDistanceFromPlayer = 0F;

    // put this here because the same weapon 
    // might have different force behind it 
    // depending on bad guy
    
    [SerializeField]
    private float attackPushbackForce = 0f;

    // amount of time that has to pass before we
    // consider another attack possibility
    
    [SerializeField]
    protected float attackDelayTimeSeconds;

    protected DateTime lastAttackTime;
    protected bool isAttacking;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        Player = GameObject.Find("Player").transform;
        lastAttackTime = DateTime.Now;
        weapon = GetComponent<BaseWeapon>();
    }

    protected virtual void UpdateAnimations()
    {
    }

    protected virtual void ApplyTransforms()
    {
        // This probably needs changed later
        // It may mess up animations, but good for now
        

        // Default move/attack flow 
        // Can be overriden as needed
        if (!isAttacking && Vector3.Magnitude(transform.position - Player.transform.position) < minDistanceFromPlayer)
        {
            // TODO:  Improve this later
            transform.LookAt(Player);
            transform.position += -transform.forward * Speed * Time.deltaTime;
        }
        else if (!isAttacking && Vector3.Magnitude(transform.position - Player.transform.position) > maxDistanceFromPlayer)
        {
            // TODO:  Improve this later
            transform.LookAt(Player);
            transform.position += transform.forward * Speed * Time.deltaTime;
        }
        else if (isAttacking)
        {
            // we're inside the sweet spot and are already attacking
        }
        else if ((DateTime.Now - lastAttackTime).Seconds > attackDelayTimeSeconds)
        {
            // We're inside the enemy's desired attack range
            // and we're not already attacking - Attack!
            Debug.Log("Start attack!!");
            isAttacking = true;
            lastAttackTime = DateTime.Now;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateAnimations();
    }

    protected virtual void FixedUpdate()
    {
        ApplyTransforms();
    }

}
