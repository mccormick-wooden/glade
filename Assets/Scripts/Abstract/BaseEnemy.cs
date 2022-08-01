using UnityEngine;
using System;
using Assets.Scripts.Abstract;
using Assets.Scripts.Damageable;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody rigidBody;
    protected NavMeshAgent agent;
    Renderer[] renderers;
    protected VelocityReporter velocityReporter;

    protected BaseWeapon weapon;

    /// <summary>
    /// The nearest beacon to the transform, located by FindClosestBeacon; 
    /// used when the enemy is in defend beacon mode, or to transition to it
    /// if the beacon has taken a lot of damage.
    /// </summary>
    public GameObject closestBeacon;

    /// <summary>
    /// Damageable of the nearest beacon
    /// </summary>
    protected BaseDamageable beaconDamageable;

    /// <summary>
    /// When in beaconDefend mode, the AI wants to patrol around the 
    /// beacon periodically looking in different directions to figure
    /// out if the player is visible to them - do that at this time
    /// </summary>
    protected DateTime nextBeaconDefenseNextPositionTime;

    /// <summary>
    /// The player, duh.
    /// </summary>
    public GameObject Player;
  
    /// <summary>
    /// Speed multiplier of the enemy
    /// </summary>
    [SerializeField]
    public float Speed;

    /// <summary>
    /// The damagable of the enemy - track HP difference between current check and 
    /// last time - use this to influence desire to run away to heal
    /// </summary>
    protected BaseDamageable damageable;

    /// <summary>
    /// The minimum distance from the player for attacking purposes - 
    /// for melee this will be small, for ranged it should be larger.
    /// If (enemy - player).magnitude is less than this distance, 
    /// the enemy will attempt to increase distance from the player when in attack mode.
    /// </summary>
    [SerializeField]
    protected float minDistanceFromPlayer = 0F;

    /// <summary>
    /// The maximum distance from the player for attacking purposes - 
    /// for melee this will be small, for ranged it should be larger.
    /// If (enemy - player).magnitude is greater than this distance, 
    /// the enemy will attempt to close the gap when in attack mode.
    /// </summary>
    [SerializeField]
    protected float maxDistanceFromPlayer = 0F;


    /// <summary>
    /// The amount of "knockback" force the enemy attack has; not used
    /// by anything (yet).
    /// </summary>    
    [SerializeField]
    private float attackPushbackForce = 0f;

    /// <summary>
    /// The amount of time that has to pass before we consider another attack possibility
    /// </summary>
    [SerializeField]
    protected float attackDelayTimeSeconds;

    /// <summary>
    /// The last time the enemy did an attack
    /// </summary>
    protected DateTime lastAttackTime;

    /// <summary>
    /// If the enemy is currently attacking
    /// </summary>
    protected bool isAttacking;

    /// <summary>
    /// Possible priorities for the AI:<br />
    ///  - AttackPlayer - try to get within min/maxDistanceFromPlayer and attack <br />
    ///  - DefendBeacon <br />
    ///  --- try to put self either between player and beacon (if player can be seen) <br />
    ///  --- patrol around the beacon (if player location is unknown) <br />
    ///  - HealAtCrystal - go to nearestCrystal; transition away if full health <br />
    ///  - HealOthers - unused at this time, but in case we get a healer enemy <br />
    ///  - NeedsRecomputed - something came up that the priority needs to be recomputed <br />
    ///  - LocatePlayer - try to find the player, only if lostPlayer == false <br />
    ///
    /// </summary>
    public enum Priority {
        AttackPlayer,
        DefendBeacon,
        HealAtCrystal,
        HealOthers,
        NeedsRecomputed,
        LocatePlayer,
        Dead
    }

    /// <summary>
    /// The current Priority of the AI (attack, defend, heal, healOthers, needsRecomputed, or LocatePlayer)
    /// </summary>
    [SerializeField]
    protected Priority priority;

    /// <summary>
    /// The minimum time between one priority being set and priority being recomputed
    /// </summary>
    [SerializeField]
    float minTimeToPriorityChange;
    
    /// <summary>
    /// The maximum time between one priority beginning and priority being recomputed
    /// </summary>
    [SerializeField]
    float maxTimeToPriorityChange;

    /// <summary>
    /// The next time to recompute the AI's priority - this may result in the same priority
    /// </summary>
    DateTime nextPriorityChangeTime;

    /// <summary>
    /// The AI's OVERALL desire to attack the player vs other priorities.
    /// These should just represent the AI's tendencies,
    /// not what it's ALWAYS going to do.  ie. an AI
    /// with a very high desireToAttackPlayer and a low
    /// desireToDefendBeacon doesn't mean it won't ever
    /// defend the beacon - just that overall it's going
    /// to highly prioritize going after the player.
    /// Game events and some random chance will influence
    /// this - ie. taking a lot of damage over a short
    /// period may add a scalar to the desireToRunAndHeal
    /// so it may still happen.
    ///
    /// The updated version of this is to just give each
    /// enemy type their own values to each of the following
    /// four fields, and they act as a relative weight to 
    /// one another..  if desiretoAttack is 4 and desire
    /// to defend beacon is 1, we can expect a 4x likelihood 
    /// of attack vs defend (other factors may influence 
    /// the AI's decision though, like a damaged beacon
    /// may be chosen more often 
    /// </summary>
    [SerializeField]
    protected float desireToAttackPlayer;

    /// <summary>
    /// The AI's OVERALL desire to defend the beacon vs other priorities.
    /// These should just represent the AI's tendencies,
    /// not what it's ALWAYS going to do.  ie. an AI
    /// with a very high desireToAttackPlayer and a low
    /// desireToDefendBeacon doesn't mean it won't ever
    /// defend the beacon - just that overall it's going
    /// to highly prioritize going after the player.
    /// Game events and some random chance will influence
    /// this - ie. taking a lot of damage over a short
    /// period may add a scalar to the desireToRunAndHeal
    /// so it may still happen.
    ///
    /// The updated version of this is to just give each
    /// enemy type their own values to each of the following
    /// four fields, and they act as a relative weight to 
    /// one another..  if desiretoAttack is 4 and desire
    /// to defend beacon is 1, we can expect a 4x likelihood 
    /// of attack vs defend (other factors may influence 
    /// the AI's decision though, like a damaged beacon
    /// may be chosen more often 
    /// </summary>
    [SerializeField]
    protected float desireToDefendBeacon;

    /// <summary>
    /// The AI's OVERALL desire to run and heal vs other priorities.
    /// These should just represent the AI's tendencies,
    /// not what it's ALWAYS going to do.  ie. an AI
    /// with a very high desireToAttackPlayer and a low
    /// desireToDefendBeacon doesn't mean it won't ever
    /// defend the beacon - just that overall it's going
    /// to highly prioritize going after the player.
    /// Game events and some random chance will influence
    /// this - ie. taking a lot of damage over a short
    /// period may add a scalar to the desireToRunAndHeal
    /// so it may still happen.
    ///
    /// The updated version of this is to just give each
    /// enemy type their own values to each of the following
    /// four fields, and they act as a relative weight to 
    /// one another..  if desiretoAttack is 4 and desire
    /// to defend beacon is 1, we can expect a 4x likelihood 
    /// of attack vs defend (other factors may influence 
    /// the AI's decision though, like a damaged beacon
    /// may be chosen more often 
    /// </summary>
    [SerializeField]
    protected float desireToRunAndHeal;

    /// <summary>
    /// The AI's OVERALL desire to heal others vs other priorities.
    /// These should just represent the AI's tendencies,
    /// not what it's ALWAYS going to do.  ie. an AI
    /// with a very high desireToAttackPlayer and a low
    /// desireToDefendBeacon doesn't mean it won't ever
    /// defend the beacon - just that overall it's going
    /// to highly prioritize going after the player.
    /// Game events and some random chance will influence
    /// this - ie. taking a lot of damage over a short
    /// period may add a scalar to the desireToRunAndHeal
    /// so it may still happen.
    ///
    /// The updated version of this is to just give each
    /// enemy type their own values to each of the following
    /// four fields, and they act as a relative weight to 
    /// one another..  if desiretoAttack is 4 and desire
    /// to defend beacon is 1, we can expect a 4x likelihood 
    /// of attack vs defend (other factors may influence 
    /// the AI's decision though, like a damaged beacon
    /// may be chosen more often 
    /// </summary>
    [SerializeField]
    protected float desireToHealOthers;


    /// <summary>
    /// The distance by which the priority automatically becomes attack player
    /// </summary>
    protected float autoAttackPlayerDistanceToBeacon;


    /// <summary>
    /// The crystalManager object of the scene - locates the nearest crystal from this.
    /// </summary>
    [SerializeField]
    protected CrystalManager crystalManager;

    /// <summary>
    /// The crystal nearest to the enemy - will run to these to heal
    /// </summary>
    protected GameObject closestCrystal;

    /// <summary>
    /// The amount of time in seconds that it takes for the enemy to 
    /// fully appear when it's first spawned
    /// </summary>
    [SerializeField]
    float timeToFullyAppear;

    /// <summary>
    /// The current alpha (transparency) of the enemy materials [0f,1f]
    /// </summary>
    float currentFade = 0f;

    /// <summary>
    /// Whether or not the AI can currently see the player
    /// </summary>
    bool canSeePlayer = false;

    /// <summary>
    /// The position where the AI last saw the player
    /// </summary>
    Vector3 lastSeenPlayerPosition;

    /// <summary>
    /// The LayerMask which defines what the raycast hits when looking for the player
    /// - should be set to most things other than enemies...  assume they chatter among 
    /// themselves if they see the player :)
    /// </summary>
    [SerializeField]
    LayerMask lookForPlayerMask;

    /// <summary>
    /// Have we visited the lastSeenPlayerPosition and still couldn't see them?
    /// </summary>
    bool lostPlayer = true;

    /// <summary>
    /// Const names for the animation stuff
    /// </summary>
    protected const string WALK_FORWARD = "Walk Forward";
    protected const string WALK_BACKWARD = "Walk Backward";
    protected const string RUN_FORWARD = "Run Forward";
    protected const string RUN_BACKWARD = "Run Backward";
    protected const string STRAFE_LEFT = "Strafe Left";
    protected const string STRAFE_RIGHT = "Strafe Right";
    protected const string MELEE_ATTACK = "Melee Attack";

    protected const string BITE_ATTACK = "Bite Attack";
    protected const string BITE_ATTACK_LOW = "Bite Attack Low";
    protected const string BITE_ATTACK_HIGH = "Bite Attack High";
    protected const string STING_ATTACK = "Sting Attack";

    protected const string CHARGING = "Charging";
    protected const string PROJECTILE_ATTACK = "Projectile Attack";
    protected const string PROJECTILE_COMBO_ATTACK = "Projectile Combo Attack";
    protected const string CAST_SPELL = "Cast Spell";
    protected const string DEFEND = "Defend";
    protected const string TAKE_DAMAGE = "Take Damage";
    protected const string DIE = "Die";

    /// <summary>
    /// Capsule collider on the enemy
    /// </summary>
    private new Collider collider;

    /// <summary>
    /// Thought bubble so the enemy can display what it's thinking
    /// </summary>
    ThoughtBubble thoughtBubble;

    public string EnemyId => $"{GetType()}:{gameObject.name}:{gameObject.GetInstanceID()}";

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        Player = GameObject.Find("PlayerModel");
        lastAttackTime = DateTime.Now;
        damageable = GetComponent<BaseDamageable>();
        priority = Priority.NeedsRecomputed;
        agent = GetComponent<NavMeshAgent>();
        autoAttackPlayerDistanceToBeacon = 0f;
        crystalManager = GameObject.Find("CrystalManager")?.GetComponent<CrystalManager>();
        renderers = GetComponentsInChildren<Renderer>();
        nextBeaconDefenseNextPositionTime = DateTime.Now;
        collider = GetComponent<Collider>();

        velocityReporter = GetComponent<VelocityReporter>();
        if (velocityReporter == null)
        {
            velocityReporter = gameObject.AddComponent<VelocityReporter>();
            velocityReporter.smoothingTimeFactor = 0.5f; // I guess? 
        }
        
        thoughtBubble = transform.Find("ThoughtBubble").GetComponent<ThoughtBubble>();
    }

    ///////////////////////////////////////////////////////
    ///
    /// Materials
    ///
    ///////////////////////////////////////////////////////


    /// <summary>
    /// Mades the model change its RenderMode to 'Fade'
    /// </summary>
    protected void SetAllMaterialsToFade()
    {
        for (int r = 0; r < renderers.Length; r++)
        {
            Material[] materials = renderers[r].materials;
            for (int i = 0; i < materials.Length; i++)
            {
                Material m = materials[i];

                // These settings are from the StandardShader from Unity:
                //   - https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/StandardShaderGUI.cs
                // This is what the Unity editor does behind the scenes when you change the render mode to "Fade"
                // and there's no one shot way to change it programmatically on the fly in code without just
                // duplicating what the editor does.

                m.SetOverrideTag("RenderType", "Transparent");
                m.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m.SetFloat("_ZWrite", 0.0f);
                m.DisableKeyword("_ALPHATEST_ON");
                m.EnableKeyword("_ALPHABLEND_ON");
                m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                Color c = m.color;
                c.a = currentFade;
                m.color = c;
            }
        }
    }

    /// <summary>
    /// Increases the alpha of all the materials on the model
    /// by deltaTime / timeToFullyAppear, and if the model
    /// is at fade == 1.0, set the model to opaque (don't waste
    /// render energy on checking transparency of opaque objects)
    /// </summary>
    void FadeMaterialsIn()
    {
        currentFade += Time.deltaTime / timeToFullyAppear;
        currentFade = Math.Min(currentFade, 1.0f);

        for (int r = 0; r < renderers.Length; r++)
        {
            Material[] materials = renderers[r].materials;
            for (int i = 0; i < materials.Length; i++)
            {
                Material m = materials[i];

                if (currentFade >= 1.0f)
                {
                    // These settings are from the StandardShader from Unity:
                    //   - https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/StandardShaderGUI.cs
                    // This is what the Unity editor does behind the scenes when you change the render mode to "Opaque"
                    // and there's no one shot way to change it programmatically on the fly in code without just
                    // duplicating what the editor does.

                    // Change back to opaque mode once fading in is done so that we don't waste performance 
                    // on transparency effects (even with 1.0 alpha) 

                    m.SetOverrideTag("RenderType", "");
                    m.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                    m.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                    m.SetFloat("_ZWrite", 1.0f);
                    m.DisableKeyword("_ALPHATEST_ON");
                    m.DisableKeyword("_ALPHABLEND_ON");
                    m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    m.renderQueue = 3000;
                    continue;
                }

                 Color c;
                 if (m.HasProperty(Shader.PropertyToID("color")))
                 {
                    c = m.color;
                    c.a = currentFade;
                    m.color = c;
                 }
            }
        }
    }



    protected virtual void UpdateAnimations()
    {

    }


    /////////////////////////////////////////////////////////////
    ///
    ///  Locators - find crystal, beacon, player, etc.
    ///
    /////////////////////////////////////////////////////////////

    /// <summary>
    /// Locates the closest beacon to the transform and sets beacon accordingly - iterate through beaconspawner's beacon managers to find it
    /// </summary>
    protected virtual void FindClosestBeacon()
    {
        GameObject beaconSpawner = GameObject.Find("BeaconSpawner");

        if (!beaconSpawner)
            return;

        float nearestBeaconDistance = float.MaxValue;
        GameObject nearestBeacon = null;

        foreach (Transform beaconManager in beaconSpawner.transform)
        {
            // crashed beacon should always be the only child of the manager
            if (beaconManager.childCount == 0)
                continue;

            GameObject crashedBeacon = beaconManager.GetChild(0)?.gameObject;

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

        closestBeacon = nearestBeacon;

        if (closestBeacon)
            beaconDamageable = closestBeacon.GetComponent<BaseDamageable>();
        else
            beaconDamageable = null;
    }

    /// <summary>
    /// Find's the crystal nearest to the transform and sets nearestCrystal accordingly
    /// </summary>
    protected virtual void FindNearestCrystal()
    {
        if (!crystalManager)
            return;

        closestCrystal = crystalManager.FindNearestCrystal(transform.position);
    }

    /// <summary>
    /// Try to determine if the player is visible from the transform -
    /// does two raycasts (from top of transform to player.center and player.top)
    /// to determine visiblity; sets canSeePlayer and lostPlayer accordingly.
    /// </summary>
    protected void CheckIfPlayerIsVisible()
    {
        Vector3 topOfTransform = GetComponent<Collider>().bounds.max;

        Vector3 topOfPlayer = Player.GetComponent<CapsuleCollider>().bounds.max;
        Vector3 centerOfPlayer = Player.GetComponent<CapsuleCollider>().bounds.center;

        Vector3 topToTopVector = topOfPlayer - topOfTransform;
        Vector3 topToCenterVector = centerOfPlayer - topOfTransform;

        RaycastHit topToTopHitInfo;
        RaycastHit topToCenterHitInfo;

        // We can add more raycasts if we find we need it, but probably unncessary 
        Physics.Raycast(topOfTransform, topToTopVector, out topToTopHitInfo, 100f, lookForPlayerMask);
        Physics.Raycast(topOfTransform, topToCenterVector, out topToCenterHitInfo, 100f, lookForPlayerMask);

        // Debug just to make sure the rays are the way we want
        Debug.DrawRay(topOfTransform, topToTopVector, Color.red);
        Debug.DrawRay(topOfTransform, topToCenterVector, Color.blue);

        if ((!topToTopHitInfo.transform || topToTopHitInfo.transform != Player.transform) && (!topToCenterHitInfo.transform || topToCenterHitInfo.transform != Player.transform))
        {
            canSeePlayer = false;
        }
        else
        {
            canSeePlayer = true;
            lostPlayer = false;
            lastSeenPlayerPosition = Player.transform.position;
        }
    }

    /// <summary>
    /// Calls each of CheckIfPlayerIsVisible, FindClosestBeacon, FindNearestCrystal
    /// </summary>
    protected void FindNearestSceneElements()
    {
        CheckIfPlayerIsVisible();
        FindClosestBeacon();
        FindNearestCrystal();
    }











    /// <summary>
    /// Handle any AI movement or attacks based on the current priority
    /// </summary>
    protected virtual void ApplyTransforms()
    {
        switch (priority)
        {
            case Priority.AttackPlayer:
                TryToGetToAttack();
                break;
            case Priority.DefendBeacon:
                if (closestBeacon == null)
                {
                    priority = Priority.NeedsRecomputed;
                    break;
                }
                TryToDefendBeacon();
                break;
            case Priority.HealAtCrystal:
                TryToGoToCrystal();
                break;
            case Priority.HealOthers:
                TryToHealOthers();
                break;
            case Priority.LocatePlayer:
                TryToLocatePlayer();
                break;
        }
    }


    /// <summary>
    /// Figure out what the AI wants to do at this current moment.  Generally, it sticks
    /// with the same as what it's been doing (why mess with success) but also may change
    /// suddenly depending on what's happened recently.
    /// </summary>
    protected void DeterminePriority()
    {
        if (damageable.IsDead)
        {
            priority = Priority.Dead;
            return;
        }

        bool couldSeePlayerLastFrame = canSeePlayer;

        // First, we need to know what's nearby and what we can see
        FindNearestSceneElements();

        // don't update priority if we're in the middle of an attack,
        if (isAttacking)
        {
            return;
        }

        // calculate some things to determine what even makes sense to try to do

        // attack is possible IF we can see the player OR we have not lost the player yet
        bool attackPlayerPossible = canSeePlayer;// || !lostPlayer;

        // defend beacon is possible IF we have a beacon to defend
        bool defendBeaconPossible = closestBeacon;

        // run and heal is possible IF we have a crystal to heal at
        bool runAndHealPossible = closestCrystal;

        // locate player is possible IF we cannot see the player AND we have not lost them
        bool locatePlayerPossible = !canSeePlayer && !lostPlayer;

        // not used right now, maybe later
        bool healOthersPossible = false;

        // check if it's no longer possible to do what we're doing
        if ((priority == Priority.AttackPlayer && !attackPlayerPossible)
            || (priority == Priority.DefendBeacon && !defendBeaconPossible)
            || (priority == Priority.HealAtCrystal && !runAndHealPossible)
            || (priority == Priority.LocatePlayer && !locatePlayerPossible)
            || (couldSeePlayerLastFrame != canSeePlayer)) // if we couldn't see the player and can now, update!
            priority = Priority.NeedsRecomputed;


        // If we've gotten this far and priority != recompute,
        // and we haven't reached the next priority change time (prevents spastic changing priorities)
        // then just return without evaluating
        if (DateTime.Now < nextPriorityChangeTime && priority != Priority.NeedsRecomputed)
            return;


        // Looks like it's time for a recompute:


        // Compute all of our weights -
        float attackPlayerWeight = Random.Range(0f, desireToAttackPlayer);
        float defendBeaconWeight = 0;
        float damageRunInfluence = 1f - damageable.CurrentHp / damageable.MaxHp;  // 0% chance at full HP
        float runAwayWeight = 0;
        float healOthersWeight = Random.Range(0, desireToHealOthers);


        // if we have full HP and priority is heal, switch to something else, no point in it staying at crystal
        if (priority == Priority.HealAtCrystal && damageRunInfluence < .01)
            priority = Priority.NeedsRecomputed;

        // first check if the beacon needs saving
        if (closestBeacon && beaconDamageable)
        {
            float beaconHealthInfluence = beaconDamageable.CurrentHp / beaconDamageable.MaxHp;
            defendBeaconWeight = Random.Range(0f, desireToDefendBeacon) * beaconHealthInfluence; // increase desire to defend beacon based on dmg
        }

        // Check out desire to run and heal
        if (closestCrystal)
        {
            runAwayWeight = Random.Range(0f, desireToRunAndHeal) * damageRunInfluence; // low desire to run at full health
        }
        

        if (attackPlayerWeight > defendBeaconWeight 
            && attackPlayerWeight > runAwayWeight 
            && attackPlayerWeight > healOthersWeight) // if we know where the player is, attack them
        {
            if (attackPlayerPossible)
                priority = Priority.AttackPlayer;
            else if (locatePlayerPossible)
                priority = Priority.LocatePlayer;
            else if (defendBeaconPossible)
                priority = Priority.DefendBeacon;
            else
                priority = Priority.NeedsRecomputed;
        }
        else if (defendBeaconPossible
            && defendBeaconWeight > attackPlayerWeight
            && defendBeaconWeight > runAwayWeight
            && defendBeaconWeight > healOthersWeight)
        {
            priority = Priority.DefendBeacon;
        }
        else if (runAndHealPossible
            && runAwayWeight > attackPlayerWeight
            && runAwayWeight > healOthersWeight
            && runAwayWeight > defendBeaconWeight)
        {
            priority = Priority.HealAtCrystal;
        }
        else if (healOthersPossible
            && healOthersWeight > attackPlayerWeight
            && healOthersWeight > defendBeaconWeight
            && healOthersWeight > runAwayWeight)
        {
            priority = Priority.HealOthers;
        }
        else
        {
            // shouldn't really need this, but just covering bases
            priority = Priority.NeedsRecomputed;
        }

        // we did a change, so figure out the next time 
        float secondsToNextChangeTime = Random.Range(minTimeToPriorityChange, maxTimeToPriorityChange);

        // record the time change
        nextPriorityChangeTime = DateTime.Now.AddSeconds(secondsToNextChangeTime);
        thoughtBubble.ShowPriority(priority);
    }


    /// <summary>
    /// Let the agent try to figure out an intercept position for the player based on their velocity
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="targetVelocity"></param>
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

    /// <summary>
    /// Let the agent try to figure out an intercept position for a gameobject
    /// </summary>
    /// <param name="targetPosition"></param>
    private void setMovingTargetWaypoint(GameObject targetPosition)
    {
        Vector3 waypointVelocity = targetPosition.GetComponent<VelocityReporter>() ? targetPosition.GetComponent<VelocityReporter>().velocity : Vector3.zero;
        float waypointMagnitude = waypointVelocity.magnitude;
       
        setMovingTargetWaypoint(targetPosition.transform.position, waypointVelocity);
    }

    /// <summary>
    /// If attack priority was highest, try to attack the player
    /// </summary>
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


    protected virtual void TryToDefendBeaconFromPlayer()
    {
        agent.isStopped = false;

        // if the player is too close to the beacon or player, 
        // the enemy should just jump to attack player 
        Vector3 beaconToPlayerVector = closestBeacon.transform.position - Player.transform.position;
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

        Vector3 desiredPosition = closestBeacon.transform.position - (beaconToPlayerVector) * (defendBeaconDistance / beaconToPlayerDistance);

        NavMeshHit hit;
        bool foundValid = NavMesh.SamplePosition(desiredPosition, out hit, 1f, NavMesh.AllAreas);

        // just assume hit is okay for now, worry about wtf to do if not later        
        Vector3 closestToDesiredPosition = hit.position;

        bool blocked = NavMesh.Raycast(transform.position, closestToDesiredPosition, out hit, NavMesh.AllAreas);

        // again, just assume good for now, figure out wtf to do if not later
        agent.SetDestination(desiredPosition);
    }

    protected virtual void TryToDefendBeaconAndWatchForPlayer()
    {
        if (!closestBeacon)
            return;

        if (DateTime.Now < nextBeaconDefenseNextPositionTime)
            return;

        float randomDistance = Random.Range(3f, 5f);
        float randomAngle = Random.Range(0f, 359f);

        float randomX = (float) Math.Cos(randomAngle) * randomDistance;
        float randomZ = (float) Math.Sin(randomAngle) * randomDistance;

        Vector3 desiredPosition = closestBeacon.transform.position + new Vector3(randomX, 0, randomZ);

        NavMeshHit hit;
        bool foundValid = NavMesh.SamplePosition(desiredPosition, out hit, 1f, NavMesh.AllAreas);

        // just assume hit is okay for now, worry about wtf to do if not later        
        Vector3 closestToDesiredPosition = hit.position;

        bool blocked = NavMesh.Raycast(transform.position, closestToDesiredPosition, out hit, NavMesh.AllAreas);

        // again, just assume good for now, figure out wtf to do if not later
        agent.SetDestination(desiredPosition);

        nextBeaconDefenseNextPositionTime = DateTime.Now.AddSeconds(5);
    }

    /// <summary>
    /// Attempt to defend the beacon - depending on if the player is visible or not, 
    /// either put the AI between the player and beacon, or if they are not just 
    /// kind of patrol around
    /// </summary>
    protected virtual void TryToDefendBeacon()
    {
        if (canSeePlayer)
            TryToDefendBeaconFromPlayer();
        else
            TryToDefendBeaconAndWatchForPlayer();
    }

    /// <summary>
    /// Attempt to go to the crystal to heal
    /// </summary>
    protected virtual void TryToGoToCrystal()
    {
        Vector3 desiredPosition = closestCrystal.transform.position;

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


    protected virtual void TryToLocatePlayer()
    {
        // shouldn't hit this
        if (canSeePlayer || lostPlayer)
        {
            priority = Priority.NeedsRecomputed;
            return;
        }
        // close enough to their position, if we can't see them, we lost them!
        else if ((transform.position - lastSeenPlayerPosition).magnitude < 1
                   && !canSeePlayer)
        {
            lostPlayer = true;
            priority = Priority.NeedsRecomputed;
            return;
        }
        else if (!canSeePlayer && !lostPlayer)
        {
            // we're lost AND we have no beacon to protect
            // this is a weird case, not sure what to do here
            // other than just go to the player's last position
            agent.SetDestination(lastSeenPlayerPosition);
        }
    }



    // Update is called once per frame
    protected virtual void Update()
    {
        if (damageable && damageable.IsDead)
        {
            rigidBody.detectCollisions = false;
            collider.enabled = false;
            transform.Find("Hovering Health Bar").gameObject.SetActive(false);
            rigidBody.detectCollisions = false;
            agent.enabled = false;
            thoughtBubble.Hide();
            thoughtBubble.enabled = false;
            this.enabled = false;
            return;
        }

        if (currentFade < 1f)
            FadeMaterialsIn();

        DeterminePriority();
        UpdateAnimations();
    }

    protected virtual void FixedUpdate()
    {
        if (damageable.IsDead)
            return;

        ApplyTransforms();
    }

    protected virtual bool IsInAttackAnimation()
    {
        var animationState = animator.GetCurrentAnimatorStateInfo(0);

        return animationState.IsName(MELEE_ATTACK)
             || animationState.IsName(PROJECTILE_ATTACK)
             || animationState.IsName(PROJECTILE_COMBO_ATTACK)
             || animationState.IsName(BITE_ATTACK)
             || animationState.IsName(BITE_ATTACK_LOW)
             || animationState.IsName(BITE_ATTACK_HIGH)
             || animationState.IsName(STING_ATTACK)
             || animationState.IsName(CAST_SPELL);
    }
}
