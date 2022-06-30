using UnityEngine;
using System;
using Assets.Scripts.Abstract;
using Assets.Scripts.Damageable;
using Random = UnityEngine.Random;
using UnityEngine.AI;


public class BaseEnemy : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidBody;
    //public GameObject playerGameObject;

    public GameObject beacon;
    protected BaseDamageable beaconDamageable;

    public GameObject Player;

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
        HealAtCrystal,
        HealOthersOthers,
        NeedsRecomputed
    }

    [SerializeField]
    protected Priority priority;

    [SerializeField]
    float minTimeToPriorityChange;
    
    [SerializeField]
    float maxTimeToPriorityChange;
    
    DateTime nextPriorityChangeTime;


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

    // The updated version of this is to just give each
    // enemy type their own values to each of the following
    // four fields, and they act as a relative weight to 
    // one another..  if desiretoAttack is 4 and desire
    // to defend beacon is 1, we can expect a 4x likelihood 
    // of attack vs defend (other factors may influence 
    // the AI's decision though, like a damaged beacon
    // may be chosen more often 

    protected float desireToAttackPlayer;
    protected float desireToDefendBeacon;
    protected float desireToRunAndHeal;
    protected float desireToHealOthers;

    // track HP difference between current check and 
    // last time - use this to influence desire to
    // run away.
    protected BaseDamageable damageable;

    protected DateTime lastPriorityChangeTime;
    protected float minTimeToPriorityChanges;

    protected float autoAttackPlayerDistanceToBeacon;

    NavMeshAgent agent;

    [SerializeField]
    protected CrystalManager crystalManager;

    protected GameObject nearestCrystal;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        Player = GameObject.Find("PlayerModel");
        lastAttackTime = DateTime.Now;
        weapon = GetComponent<BaseWeapon>();
        damageable = GetComponent<AnimateDamageable>();
        // if not animate, check for disappear
        if (!damageable)
        {
            damageable = GetComponent<DisappearDamageable>();
        }

        priority = Priority.NeedsRecomputed;
        lastPriorityChangeTime = DateTime.Now;
        // make sure to set starting priorities in


        agent = GetComponent<NavMeshAgent>();
        autoAttackPlayerDistanceToBeacon = 0f;

        crystalManager = GameObject.Find("CrystalParent").GetComponent<CrystalManager>();
    }

    protected virtual void UpdateAnimations()
    {
    }

    protected virtual void FindClosestBeacon()
    {
        GameObject beaconSpawner = GameObject.Find("BeaconSpawner");

        if (!beaconSpawner)
            return;

        float nearestBeaconDistance = float.MaxValue;
        GameObject nearestBeacon = null;

        foreach (Transform beaconManager in beaconSpawner.transform)
        {
            GameObject crashedBeacon = beaconManager.Find("CrashedBeacon")?.gameObject;

            if (!crashedBeacon)
                continue;

            float distanceToEnemy = (transform.position - crashedBeacon.transform.position).magnitude;
            float distanceToPlayer = (Player.transform.position - crashedBeacon.transform.position).magnitude;

            if (distanceToEnemy < nearestBeaconDistance)
            {
                nearestBeaconDistance = distanceToEnemy;
                nearestBeacon = crashedBeacon.gameObject;
            }
        }

        beacon = nearestBeacon;
        if (beacon != null)
        {
            beaconDamageable = beacon.GetComponent<AnimateDamageable>();
            if (beaconDamageable == null)
                beaconDamageable = beacon.GetComponent<DisappearDamageable>();
        }
        else
        {
            beaconDamageable = null;
        }
    }

    protected virtual void FindNearestCrystal()
    {
        nearestCrystal = crystalManager.FindNearestCrystal(transform.position);
    }


    protected virtual void ApplyTransforms()
    {
        switch (priority)
        {
            case Priority.AttackPlayer:
                TryToGetToAttack();
                break;
            case Priority.DefendBeacon:
                if (beacon == null)
                {
                    priority = Priority.NeedsRecomputed;
                    break;
                }
                TryToDefendBeacon();
                break;
            case Priority.HealAtCrystal:
                TryToGoToCrystal();
                break;
            case Priority.HealOthersOthers:
                TryToHealOthers();
                break;
        }
    }

    protected void DeterminePriority()
    {
        // don't update priority if we're in the middle of an attack,
        // OR we haven't reached the next priority change time (prevents spastic changing priorities)
        // OR the priority hasn't been flagged as needing computing early
        
        // alternatively, if the enemy has full HP and priority is heal,
        // switch to something else, no point in it staying at beacon longer

        float damageRunInfluence = 1f - damageable.CurrentHp / damageable.MaxHp;  // 0% chance at full HP

        if (priority == Priority.HealAtCrystal && damageRunInfluence < .01)
        {
            priority = Priority.NeedsRecomputed;
        }   

        if (isAttacking || (DateTime.Now < nextPriorityChangeTime && priority != Priority.NeedsRecomputed))
        {
            return;
        }
            
        FindClosestBeacon();
        FindNearestCrystal();


        // first check if the beacon needs saving
        float attackPlayerWeight = Random.Range(0f, desireToAttackPlayer);
        float defendBeaconWeight = 0;
        
        if (beacon)
        {
            float beaconHealthInfluence = beaconDamageable.CurrentHp / beaconDamageable.MaxHp;
            defendBeaconWeight = Random.Range(0f, desireToDefendBeacon) * beaconHealthInfluence; // increase desire to defend beacon based on dmg
        }

        float runAwayWeight = 0;

        if (nearestCrystal)
        {
            runAwayWeight = Random.Range(0f, desireToRunAndHeal) * damageRunInfluence; // low desire to run at full health
        }
        
        float healOthersWeight = Random.Range(0, desireToHealOthers);

        if (attackPlayerWeight > defendBeaconWeight 
            && attackPlayerWeight > runAwayWeight 
            && attackPlayerWeight > healOthersWeight)
        {
            priority = Priority.AttackPlayer;
        }
        else if (defendBeaconWeight > attackPlayerWeight
            && defendBeaconWeight > runAwayWeight
            && defendBeaconWeight > healOthersWeight)
        {
            priority = Priority.DefendBeacon;
        }
        else if (runAwayWeight > attackPlayerWeight
            && runAwayWeight > healOthersWeight
            && runAwayWeight > defendBeaconWeight)
        {
            priority = Priority.HealAtCrystal;
        }
        else if (healOthersWeight > attackPlayerWeight
            && healOthersWeight > defendBeaconWeight
            && healOthersWeight > runAwayWeight)
        {
            priority = Priority.HealOthersOthers;
        }
        else
        {
            // shouldn't really need this, but just covering bases
            priority = Priority.NeedsRecomputed;
        }

        float secondsToNextChangeTime = Random.Range(minTimeToPriorityChange, maxTimeToPriorityChange);
        nextPriorityChangeTime = DateTime.Now.AddSeconds(secondsToNextChangeTime);
    }


    private void setMovingTargetWaypoint(Vector3 targetPosition, Vector3 targetVelocity)
    {
        Vector3 distanceBetweenPlayerAndWaypoint = targetPosition - transform.position;
        float distanceBetweenPlayerAndWaypointMagnitutde = distanceBetweenPlayerAndWaypoint.magnitude;

        // todo - tweak this, this tends to overshoot at high velcity player coming in toward 
        float lookAheadInSeconds = distanceBetweenPlayerAndWaypointMagnitutde / agent.speed; 
        Vector3 futureExtrapolatedPosition = targetPosition + (targetVelocity * lookAheadInSeconds);

        NavMeshHit hit;
        bool blocked = NavMesh.Raycast(transform.position, futureExtrapolatedPosition, out hit, 1 << NavMesh.GetAreaFromName("Walkable"));

        if (blocked)
        {
            agent.SetDestination(targetPosition);
        }
        else
        {
            agent.SetDestination(futureExtrapolatedPosition);
        }
    }


    private void setMovingTargetWaypoint(GameObject targetPosition)
    {
        Vector3 waypointVelocity = targetPosition.GetComponent<VelocityReporter>() ? targetPosition.GetComponent<VelocityReporter>().velocity : Vector3.zero;
        float waypointMagnitude = waypointVelocity.magnitude;
       
        setMovingTargetWaypoint(targetPosition.transform.position, waypointVelocity);
    }

    protected virtual void TryToGetToAttack()
    {
        // This probably needs changed later
        // It may mess up animations, but good for now

        // Default move/attack flow 
        // Can be overriden as needed
        float distanceToPlayer = Vector3.Magnitude(transform.position - Player.transform.position);


        if (!isAttacking && (distanceToPlayer > maxDistanceFromPlayer))
        {

            agent.isStopped = false;
            setMovingTargetWaypoint(Player);
        }
        else if (!isAttacking && (distanceToPlayer < minDistanceFromPlayer))
        {
            // TODO: update this
            // if navmesh.raycast backwards can't find  
            // a valid position, iterate to find one.

            agent.isStopped = false;

            Vector3 vectorFromPlayer = -(Player.transform.position - transform.position);
            float desiredExtraDistance = minDistanceFromPlayer - distanceToPlayer;
            vectorFromPlayer *= desiredExtraDistance;
            Vector3 velocity = Player.GetComponent<VelocityReporter>().velocity;

            setMovingTargetWaypoint(transform.position + vectorFromPlayer, velocity);
        }
        else if (isAttacking)
        {
            // we're inside the sweet spot and are already attacking
            agent.SetDestination(transform.position);
            agent.isStopped = true;
        }
        else if ((DateTime.Now - lastAttackTime).Seconds > attackDelayTimeSeconds)
        {
            // We're inside the enemy's desired attack range
            // and we're not already attacking - Attack!
            transform.LookAt(Player.transform.position);

            isAttacking = true;
            lastAttackTime = DateTime.Now;
            agent.isStopped = true;
            agent.SetDestination(transform.position);
        }
    }


    // don't let it get here if player and agent aren't nearish to a beacon
    protected virtual void TryToDefendBeacon()
    {
        // if the player is too close to the beacon or player, 
        // the enemy should just jump to attack player 
        Vector3 beaconToPlayerVector = beacon.transform.position - Player.transform.position;
        float beaconToPlayerDistance = Vector3.Magnitude(beaconToPlayerVector);
        
        Vector3 enemyToPlayerVector = Player.transform.position - transform.position;
        float enemyToPlayerDistance = Vector3.Magnitude(enemyToPlayerVector);

        if (beaconToPlayerDistance < autoAttackPlayerDistanceToBeacon
            || enemyToPlayerDistance < minDistanceFromPlayer) 
        {
            priority = Priority.AttackPlayer;
            TryToGetToAttack();
        }

        // place ourselves in between the player and beacon
        float defendBeaconDistance = 3f;

        Vector3 desiredPosition = beacon.transform.position - (beaconToPlayerVector) * (defendBeaconDistance / beaconToPlayerDistance);

        NavMeshHit hit;
        bool foundValid = NavMesh.SamplePosition(desiredPosition, out hit, 1f, NavMesh.AllAreas);

        // just assume hit is okay for now, worry about wtf to do if not later        
        Vector3 closestToDesiredPosition = hit.position;

        bool blocked = NavMesh.Raycast(transform.position, closestToDesiredPosition, out hit, NavMesh.AllAreas);

        // again, just assume good for now, figure out wtf to do if not later
        agent.SetDestination(desiredPosition);
    }


    protected virtual void TryToGoToCrystal()
    {
        //Vector3 enemyToPlayerVector = Player.transform.position - transform.position;

        //Vector3 enemyToCrystalVector = Player.transform.position - nearestCrystal.transform.position;

        // simple for now, just flee player, go to enemyToPlayer + 1 each frame
        // use samplePosition to 

        //Vector3 desiredPosition = enemyToCrystalVector + enemyToCrystalVector.normalized;

        Vector3 desiredPosition = nearestCrystal.transform.position;

        NavMeshHit hit;
        bool foundValid = NavMesh.SamplePosition(desiredPosition, out hit, 1f, NavMesh.AllAreas);

        // just assume hit is okay for now, worry about wtf to do if not later        
        Vector3 closestToDesiredPosition = hit.position;

        bool blocked = NavMesh.Raycast(transform.position, closestToDesiredPosition, out hit, NavMesh.AllAreas);

        // again, just assume good for now, figure out wtf to do if not later
        agent.SetDestination(hit.position);
        agent.isStopped = false;
    }


    // assume for now only some enemies can do that, leave this blank?
    protected virtual void TryToHealOthers()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        DeterminePriority();
        UpdateAnimations();
    }

    protected virtual void FixedUpdate()
    {
        ApplyTransforms();
    }

}
