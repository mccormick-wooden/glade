using UnityEngine;
using System;
using Assets.Scripts.Abstract;
using Assets.Scripts.Damageable;
using Random = UnityEngine.Random;

public class BaseEnemy : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidBody;
    public GameObject playerGameObject;
    public GameObject beacon;
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

    protected enum Priority {
        AttackPlayer,
        DefendBeacon,
        IncreaseDistanceFromPlayer,
        HealOthersOthers
    }

    protected Priority priority;

    // The AI's OVERALL desire to do the following.
    // These should just represent the AI's tendencies,
    // not what it's ALWAYS going to do.  ie. an AI
    // with a very high desireToAttackPlayer and a low
    // desireToDefendBeacon doesn't mean it won't ever
    // defend the beacon - just that overall it's going
    // to highly prioritize going after the player.
    // Game events and some random chance will influence
    // this - ie. taking a lot of damage over a short
    // period may add a scalar to the desireToRunAndHeal
    // so it may still happen.

    // The (current) thinking is these should be clamped
    // from [0,1] each..  the sum of desire to attack/defend
    // should total 1, and the desire to run should be
    // between [0,1] as well.  

    // Desire to run should indicate the MAX chance 
    // that the AI wants to run away if it's just 
    // taken a TON of damage, otherwise the desire
    // to run drops proportionally to how much
    // health it still has left.  The rest of the
    // 1 - desireToRun is distributed between the
    // other two, according to their proportions.

    // e.g. if desire to run is 0.25, and desire to
    // attack/defend are 0.75, 0.25, if the AI has
    // lost 50% of it's health, desire to run should
    // be 0.125, and the desire to attack / defend,
    // then desires to attack/defend are reduced
    // by 0.125/2 each, leaving us with:
    // - 0.6875 desire to attack
    // - 0.1875 desire to defend
    // - 0.1250 desire to run

    protected float desireToAttackPlayer;
    protected float desireToDefendBeacon;
    protected float desireToRunAndHeal;
    protected float desireToHealOthers;

    // track HP difference between current check and 
    // last time - use this to influence desire to
    // run away.
    protected BaseDamageable damageable;
    //protected float recentHP;

    DateTime lastPriorityChangeTime;
    float minTimeToPriorityChanges;

    float autoAttackPlayerDistanceToBeacon;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        Player = GameObject.Find("Player").transform;
        lastAttackTime = DateTime.Now;
        weapon = GetComponent<BaseWeapon>();
        damageable = GetComponent<AnimateDamageable>();
        // if not animate, check for disappear
        if (!damageable)
        {
            damageable = GetComponent<DisappearDamageable>();
        }

        lastPriorityChangeTime = DateTime.Now;
        // make sure to set starting priorities in 
    }

    protected virtual void UpdateAnimations()
    {
    }

    protected virtual void ApplyTransforms()
    {
        switch (priority)
        {
            case Priority.AttackPlayer:
                TryToGetToAttack();
                break;
            case Priority.DefendBeacon:
                TryToDefendBeacon();
                break;
            case Priority.IncreaseDistanceFromPlayer:
                TryToIncreaseDistance();
                break;
            case Priority.HealOthersOthers:
                TryToHealOthers();
        }
    }


    protected virtual void TryToGetToAttack()
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


    protected virtual void TryToDefendBeacon()
    {
        // if the player is too close to the beacon or player, 
        // the enemy should just jump to attack player 
        Vector3 beaconToPlayerVector = beacon.transform.position - Player.transform.position;
        Vector3 beaconToEnemyVector = beacon.transform.position - transform.position;
        Vector3 playerToEnemyVector = Player.transform.position - transform.position;

        if (Vector3.Magnitude(beaconToPlayerVector) < autoAttackPlayerDistanceToBeacon
            || Vector3.Magnitude(playerToEnemyVector) < minDistanceFromPlayer) 
        {
            priority = Priority.AttackPlayer;
            TryToGetToAttack();
        }

        // calculate a few values first:

        float distanceToBeacon = Vector3.Magnitude(transform.position - beacon.transform.position);
        float angleBetweenPlayerBeaconAndBeaconEnemy = Vector3.Angle(beaconToPlayerVector, beaconToEnemyVector);

        Vector3 directionToMove = Vector3.zero;

        if (distanceToBeacon > 

    }



    protected virtual void DetermineAIPriority()
    {
        // override this on a case by case, AI by AI basis
        // this is a basic priority tree that will suit most,
        // be sure to only allow it to change over N second
        // time periods - don't let it jitter between 
        // priorities all over the place.  make up your mind
        // and stick to it for a bit.

        // Don't switch mid-attack
        if (!isAttacking 
              && DateTime.Now > lastPriorityChangeTime.AddSeconds(minTimeToPriorityChanges))
            return;

        float chanceOfRun = (1 + damageable.CurrentHp / damageable.MaxHp) * desireToRunAndHeal;

        // if there's no beacon, no desire to defend
        float leftOver = 1 - chanceOfRun;
        float chanceOfBeacon = beacon ? leftOver * desireToDefendBeacon : 0f;

        leftOver -= chanceOfBeacon;

        float chanceOfAttack = leftOver * desireToAttackPlayer;
        float chanceOfHealOthers = leftOver * desireToHealOthers;

        float random = Random.Range(0.0f, 1.0f);

        if (random < chanceOfAttack)
        {
            priority = Priority.AttackPlayer;
        }
        else if (random > chanceOfAttack && random < (chanceOfAttack + chanceOfBeacon))
        {
            priority = Priority.DefendBeacon;
        }
        else if (random > (chanceOfAttack + chanceOfBeacon)
            && (random < chanceOfAttack + chanceOfBeacon + chanceOfRun))
        {
            priority = Priority.IncreaseDistanceFromPlayer;
        }
        else
        {
            priority = Priority.HealOthersOthers;
        }

        lastPriorityChangeTime = DateTime.Now;
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        DetermineAIPriority();

        UpdateAnimations();
    }

    protected virtual void FixedUpdate()
    {
        ApplyTransforms();
    }

}
