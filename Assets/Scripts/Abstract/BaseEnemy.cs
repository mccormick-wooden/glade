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
        IncreaseDistanceFromPlayer,
        HealOthersOthers
    }

    [SerializeField]
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

    DateTime lastPriorityChangeTime;
    float minTimeToPriorityChanges;

    float autoAttackPlayerDistanceToBeacon;

    NavMeshAgent agent;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        Player = GameObject.Find("Player");
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
        
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void UpdateAnimations()
    {
    }

    protected virtual void ApplyTransforms()
    {
        priority = Priority.DefendBeacon;

        GameObject beaconSpawner = GameObject.Find("BeaconSpawner");

        float nearestBeaconDistance = float.MaxValue;
        GameObject nearestBeacon = null;

        foreach (Transform beaconManager in beaconSpawner.transform)
        {
            GameObject crashedBeacon = beaconManager.GetChild(0).gameObject;

            float distanceToEnemy = (transform.position - crashedBeacon.transform.position).magnitude;
            float distanceToPlayer = (Player.transform.position - crashedBeacon.transform.position).magnitude;

            if (distanceToEnemy < nearestBeaconDistance)
            {
                nearestBeaconDistance = distanceToEnemy;
                nearestBeacon = crashedBeacon;
            }
        }

        beacon = nearestBeacon;

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
                break;
        }
    }


    private void setMovingTargetWaypoint(Vector3 targetPosition, Vector3 targetVelocity)
    {
        float agentVelocityMagnitude = agent.speed;

        Vector3 distanceBetweenPlayerAndWaypoint = targetPosition - transform.position;
        float distanceBetweenPlayerAndWaypointMagnitutde = distanceBetweenPlayerAndWaypoint.magnitude;

        float angle = Mathf.Rad2Deg * Mathf.Atan2(distanceBetweenPlayerAndWaypoint.x, distanceBetweenPlayerAndWaypoint.z);
        //Debug.Log($"Angle: {angle}");

        //Debug.Log($"Distances: P->W: {distanceBetweenPlayerAndWaypoint}, Mag(P->W): {distanceBetweenPlayerAndWaypointMagnitutde}");

        float lookAheadInSeconds = distanceBetweenPlayerAndWaypointMagnitutde / agentVelocityMagnitude;

        //Debug.Log($"Looking ahead {lookAheadInSeconds}");

        Vector3 futureExtrapolatedPosition = targetPosition + (targetVelocity * lookAheadInSeconds);

        //Debug.Log($"Future extrapoloated position: {futureExtrapolatedPosition}");

        NavMeshHit hit;
        bool blocked = NavMesh.Raycast(transform.position, futureExtrapolatedPosition, out hit, 1 << NavMesh.GetAreaFromName("Walkable"));

        if (blocked)
        {
            //Debug.Log("Blocked");
            agent.SetDestination(targetPosition);
        }
        else
        {
            //Debug.Log("Not blocked");
            agent.SetDestination(futureExtrapolatedPosition);

        }
    }


    private void setMovingTargetWaypoint(GameObject targetPosition)
    {
        //Debug.Log("Updating");

        Vector3 waypointVelocity = targetPosition.GetComponent<VelocityReporter>() ? targetPosition.GetComponent<VelocityReporter>().velocity : Vector3.zero;
        float waypointMagnitude = waypointVelocity.magnitude;

        //Debug.Log($"Waypoint velocity: {waypointVelocity}, magnitude: {waypointMagnitude}");
        
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
            //transform.LookAt(Player.transform.position);

            // we're inside the sweet spot and are already attacking
            agent.SetDestination(transform.position);
            agent.isStopped = true;
        }
        else if ((DateTime.Now - lastAttackTime).Seconds > attackDelayTimeSeconds)
        {
            // We're inside the enemy's desired attack range
            // and we're not already attacking - Attack!
            //Debug.Log("Start attack!!");
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

        //Vector3 desiredPosition = Player.transform.position + (beaconToPlayerVector) * (defendBeaconDistance / beaconToPlayerDistance);
        Vector3 desiredPosition = beacon.transform.position - (beaconToPlayerVector) * (defendBeaconDistance / beaconToPlayerDistance);


        NavMeshHit hit;
        bool foundValid = NavMesh.SamplePosition(desiredPosition, out hit, 1f, NavMesh.AllAreas);

        // just assume hit is okay for now, worry about wtf to do if not later        
        Vector3 closestToDesiredPosition = hit.position;

        bool blocked = NavMesh.Raycast(transform.position, closestToDesiredPosition, out hit, NavMesh.AllAreas);

        // again, just assume good for now, figure out wtf to do if not later
        agent.SetDestination(desiredPosition);
    }


    protected virtual void TryToIncreaseDistance()
    {
        Vector3 enemyToPlayerVector = Player.transform.position - transform.position;

        // simple for now, just flee player, go to enemyToPlayer + 1 each frame
        // use samplePosition to 

        Vector3 desiredPosition = enemyToPlayerVector + enemyToPlayerVector.normalized;

        NavMeshHit hit;
        bool foundValid = NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas);

        // just assume hit is okay for now, worry about wtf to do if not later        
        Vector3 closestToDesiredPosition = hit.position;

        bool blocked = NavMesh.Raycast(transform.position, closestToDesiredPosition, out hit, NavMesh.AllAreas);

        // again, just assume good for now, figure out wtf to do if not later
        agent.SetDestination(desiredPosition);
    }


    // assume for now only some enemies can do that, leave this blank?
    protected virtual void TryToHealOthers()
    {

    }


    protected virtual void DetermineAIPriority()
    {
        // override this on a case by case, AI by AI basis
        // this is a basic priority tree that will suit most,
        // be sure to only allow it to change over N second
        // time periods - don't let it jitter between 
        // priorities all over the place.  make up your mind
        // and stick to it for a bit.



        /*
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
        */
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
