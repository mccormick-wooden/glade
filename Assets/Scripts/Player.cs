using System;
using System.Collections;
using PlayerBehaviors;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    // Public
    public float CameraHorizontalAngleChange { get; private set; }
    public float CameraVerticalAngleChange { get; private set; }
    public Transform PlayerTransform => transform;
    public float RotationAngle => rotationAngle;

    // Controls related stuff
    private CharacterPlayerControls controls;
    private float verticalInput;
    private float horizontalInput;
    // New player orientation and movement magnitude based on player input. We
    // only care about changes in the horizontal plane (thus, zero out the
    // Y/Vertical axis).
    private Vector3 playerOrientation => new Vector3(horizontalInput, 0f, verticalInput);
    private float movementMagnitude => playerOrientation.magnitude;

    // Controls how fast the character moves and turns.
    public float strafeMovementSpeedModifier = 0.6f;
    public float movementSpeed = 1.2f;
    public float rotationSmoothTime = 0.1f;
    float rotationVelocity = 0f;

    // Used for slope handling and falling off cliffs.
    Vector3 terrainSize;
    float downpullForce = 0f;
    float horizontalMultiplier = 1f;
    bool hasLanded = true;
    public float fallMomentumFactor = 1.35f;

    // Character movement will be relative to this camera.
    public PlayerLockOnCamera playerLockOnCamera;
    private Transform mainCameraTransform;

    // Action related stuff
    private float rotationAngle;

    // Jumping
    private bool isJumping;
    private bool isGrounded;
    private bool isFalling;
    private float fallingTimeout = 5f;
    private bool applyInitialFallMomentum = true;

    private bool tryPickup;

    // Components of the player GameObject we reference
    private Rigidbody rigidBody;
    private Animator animator;

    // Stuff to help us know when we're in contact with the ground
    [SerializeField] private LayerMask whatIsGround;

    public float playerSpeedForce;
    
    CapsuleCollider capsuleCollider;
    
    public PlayerCombat PlayerCombat { get; private set; }
    private PlayerWeaponManager playerWeaponManager;

    // These refer to the scriptable objects that should be used as primary and secondary
    // Scriptable objects let us separate the Player prefab from the weapons and lets us do things 
    // like swap weapons at run-time if we want to produce different effects, models, damage, animations, etc
    public PlayerWeapon primaryWeapon;
    private PlayerWeapon secondaryWeapon;
    
    private static readonly int HorizontalInput = Animator.StringToHash("horizontalInput");
    private static readonly int VerticalInput = Animator.StringToHash("verticalInput");
    private static readonly int IsStrafing = Animator.StringToHash("isStrafing");

    Transform touchToPickup;
    Transform leftHand;
    FruitDetector fruitDetector;
    Transform fruitInHand;
    Transform targetFruitForPickup;

    [SerializeField]
    Transform leftFoot;
    
    [SerializeField]
    Transform rightFoot;

    private bool leftFootDown = false;
    private bool rightFootDown = false;
    public float maxDistanceToGroundForFootstep;

    private void Awake()
    {
        # region CombatAwake

        PlayerCombat = GetComponent<PlayerCombat>();
        Utility.LogErrorIfNull(PlayerCombat, "playerCombat",
            "Player game object must have a PlayerCombat component");
        
        playerWeaponManager = GetComponent<PlayerWeaponManager>();
        Utility.LogErrorIfNull(playerWeaponManager, "playerWeaponManager",
            "Player game object must have a PlayerWeaponManager component");

        #endregion
        
        GetTerrainInfo();
        GetCamera();
        GatherComponents();
        SetPlayerPhysicalProperties();
        SetupControls();
        SetupAttackControls();
    }

    // Start is called before the first frame update
    private void Start()
    {
        # region CombatStart

        Utility.LogErrorIfNull(primaryWeapon, "primaryWeapon",
            "Please add a Weapon to the primaryWeapon variable on Player in the Editor");

        // Uncomment when we're ready for an offhand weapon
        // Utility.LogErrorIfNull(secondaryWeapon, "secondaryWeapon",
        // "Please add a Weapon to the secondaryWeapon variable on Player in the Editor");

        if (primaryWeapon != null)
        {
            playerWeaponManager.EquipWeaponSlot(primaryWeapon, false);
        }

        if (secondaryWeapon != null)
        {
            playerWeaponManager.EquipWeaponSlot(secondaryWeapon, true);
        }

        #endregion

        animator.speed = movementSpeed;
        animator.applyRootMotion = true;
        isGrounded = true;
        isJumping = false;
        isFalling = false;
        fallingTimeout = 5f;
        applyInitialFallMomentum = true;
    }

    private void FixedUpdate()
    {
        ApplyForcesAndDrag();
        ApplyTransforms();

        animator.SetFloat(HorizontalInput, horizontalInput);
        animator.SetFloat(VerticalInput, verticalInput);
    }

    private void ApplyForcesAndDrag()
    {
        // just a dummy thing for now because we don't have jump

        var isJumpable = false;
        isGrounded = CheckGroundNear(transform.position, out isJumpable);

        if (isJumping && isGrounded)
        {
            rigidBody.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
            isJumping = false;
        }

        // ---
        var animState = animator.GetCurrentAnimatorStateInfo(0);

        if (isFalling)
        {
            // Keep track of how long we've been falling for.
            fallingTimeout -= Time.fixedDeltaTime;
        }

        // Transition to landing if:
        //  1) We're falling and near ground check yields true. OR
        //  2) Falling state times out.
        if ((animState.IsName("Falling") && isGrounded) || (fallingTimeout <= 0f))
        {
            isFalling = false;
            fallingTimeout = 5f;
            applyInitialFallMomentum = true;
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsGrounded", true);
        }

        if (animState.IsName("MovementTree"))
        {
            // Laxer ground check. Hopefully this will only get trigger when on
            // actual cliffs.
            var halfHeight = capsuleCollider.height / 2f;
            var groundInterceptRayStart = transform.position + new Vector3(0, halfHeight, 0);
            var groundInterceptRayLength = capsuleCollider.height * 1.8f;

            var myIsGrounded = Physics.Raycast(
                    groundInterceptRayStart,
                    Vector3.down,
                    groundInterceptRayLength,
                    whatIsGround);

            // Determine whether the player is going uphill or downhill:
            // 1. Get the player's position on the map (horizontal plane).
            float normalizedXposition = (transform.position.x / terrainSize.x);
            float normalizedZposition = (transform.position.z / terrainSize.z);
            // 2. Get the terrain's Normal on that point.
            Vector3 groundNormal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(
                    normalizedXposition, normalizedZposition);
            // 3. Get the angular difference between the terrain's Normal
            // and the player's forward component.
            // Note: Use an offset of -90 degrees to make a perfect
            // alignment equal to 0.
            // Uphill: positive angles.
            // Downhill: negative angles.
            float slopeAngle = (
                    Vector3.Angle(groundNormal, transform.forward) - 90f);
            //Debug.Log("groundNormal: " + groundNormal);
            //Debug.Log("slopeAngle: " + slopeAngle);

            // Transition to falling state when off ground and very steep
            // downward slopes.
            if (!myIsGrounded && (slopeAngle < -40f))
            {
                //Debug.Log("Airborne");
                isFalling = true;
                animator.SetBool("IsGrounded", false);
                animator.SetBool("IsFalling", true);
            }
        }

        // Apply momentum only the first time.
        if (animState.IsName("Jump") && applyInitialFallMomentum)
        {
            rigidBody.AddForce(
                new Vector3(
                    fallMomentumFactor * rigidBody.velocity.x,
                    rigidBody.velocity.y,
                    fallMomentumFactor * rigidBody.velocity.z),
                ForceMode.VelocityChange);
            applyInitialFallMomentum = false;
        }

        if (animState.IsName("Landing"))
        {
            UpdateControlState(false);
            rigidBody.velocity = Vector3.zero;
        }
        else
        {
            UpdateControlState(true);
        }
    }

    private void ApplyTransforms()
    {
        rigidBody.drag = isGrounded ? 5 : 0;
    }
    
    private void GetTerrainInfo()
    {
        terrainSize = Terrain.activeTerrain.terrainData.size;
        if (terrainSize.Equals(Vector3.zero))
        {
            Debug.LogError("Terrain Data not found");
        }
    }

    private void GetCamera()
    {
        playerLockOnCamera = GetComponentInChildren<PlayerLockOnCamera>();
        mainCameraTransform = GameObject.Find("CameraParent/MainCamera")?.transform;
        Utility.LogErrorIfNull(mainCameraTransform, "mainCameraTransform",
            "Player could not find the mainCameraTransform");
    }

    void SetPlayerPhysicalProperties()
    {
        rigidBody.mass = 85;
        rigidBody.drag = 5;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void SetupControls()
    {
        controls = new CharacterPlayerControls();

        controls.Gameplay.Move.performed += ctx =>
        {
            Vector2 leftStick = ctx.ReadValue<Vector2>();
            horizontalInput = leftStick.x;
            verticalInput = leftStick.y;

            // Update animation.
            animator.SetFloat("Speed", movementMagnitude);

            // Ignore any negligible motion.
            if (movementMagnitude < 0.1)
            {
                horizontalInput = 0f;
                verticalInput = 0f;
            }
        };
    }

    private void SetupAttackControls()
    {
        controls.Gameplay.LockOnToggle.performed += ctx => PlayerCombat.ToggleLockOn();
        controls.Gameplay.LockOnCycleLeft.performed += ctx => PlayerCombat.HandleLockOnCycle(true);
        controls.Gameplay.LockOnCycleRight.performed += ctx => PlayerCombat.HandleLockOnCycle(false);
        controls.Gameplay.Slash.performed += ctx => PlayerCombat.PerformSlashAttack(primaryWeapon);
        controls.Gameplay.SpecialAttack.performed += ctx => PlayerCombat.PerformSpecialAttack(primaryWeapon);

        controls.Gameplay.Pickup.performed += ctx => { tryPickup = true; };
    }

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new CharacterPlayerControls();
        }
        controls.Gameplay.Enable();
    }
    

    private void OnDisable()
    {
        if (controls == null)
        {
            controls = new CharacterPlayerControls();
        }
        controls.Gameplay.Disable();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            hasLanded = true;
        }
    }

    void GatherComponents()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rigidBody = GetComponent<Rigidbody>();
        //sword = GameObject.Find("Sword").GetComponent<Sword>();
        fruitDetector = GameObject.Find("FruitDetector")?.GetComponent<FruitDetector>();
        leftHand = GameObject.Find("HoldInLeftHand").transform;
        touchToPickup = GameObject.Find("TouchToPickupPosition").transform;
    }


    public bool CheckGroundNear(
        Vector3 charPos,
        out bool isJumpable
    )
    {
        const float MAX_JUMPABLE_ANGLE = 45f;
        const float RAY_ORIGIN_OFFSET = 1f;
        const float RAY_DEPTH = .1f;

        bool ret = false;
        bool _isJumpable = false;

        float totalRayLen = RAY_ORIGIN_OFFSET + RAY_DEPTH;

        Ray ray = new Ray(charPos + Vector3.up, Vector3.down);

        Debug.DrawRay(charPos + Vector3.up * RAY_ORIGIN_OFFSET, Vector3.down, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(ray, totalRayLen, whatIsGround);
        RaycastHit groundHit = new RaycastHit();

        foreach (RaycastHit hit in hits)
        {
            //if (hit.collider.gameObject.CompareTag("Walkable"))
            //{

            ret = true;

            groundHit = hit;

            _isJumpable = Vector3.Angle(Vector3.up, hit.normal) < MAX_JUMPABLE_ANGLE;

            break; //only need to find the ground once

            //}
        }

        isJumpable = _isJumpable;

        return ret;
    }
    
    private void OnAnimatorMove()
    {
        // The player movement is relative to the position of the camera when not locked on:
        //
        //  - Forward   : Player moves in the direction the camera is facing
        //                (i.e. away from the camera).
        //  - Backward  : Player moves towards the camera.
        //  - Left      : Player moves towards the camera's left.
        //  - Right     : Player moves towards the camera's right.

        /*
         * While locked on we should expect to always face the lock on target and our movements to be relative to it
         */
        var animState = animator.GetCurrentAnimatorStateInfo(0);

        if (animState.IsName("Sheathe") || animState.IsName("Picking Up") || animState.IsName("DrawSword"))
            return;

        if (movementMagnitude >= 0.1)
        {
            CheckFootSounds();

            if (PlayerCombat.isLockingOn)
            {
                // Look at the target
                var lockOnTarget = PlayerCombat.currentLockedOnTarget.transform.position;
                transform.LookAt(new Vector3(lockOnTarget.x, transform.position.y, lockOnTarget.z));
            }
            else
            {
                // ROTATION --------------------------------------------------------

                // Compute how much the player needs to rotate around its own
                // Y/Vertical axis to align itself with the desired orientation.
                var targetAngle = Mathf.Rad2Deg *
                                  Mathf.Atan2(playerOrientation.x, playerOrientation.z);

                // Make the rotation relative to the camera's Y/Vertical position.
                targetAngle += mainCameraTransform.eulerAngles.y;

                // Smooth the angle transition.
                var smoothedAngle = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y, // Original player orientation.
                    targetAngle, // Desired player orientation.
                    ref rotationVelocity,
                    rotationSmoothTime);

                // Compute the player's angular and linear movements:
                // Rotation: Around its own Y/Vertical axis.
                var sharpRotation = Quaternion.Euler(0f, targetAngle, 0f);
                var smoothedRotation = Quaternion.Euler(0f, smoothedAngle, 0f);

                // Apply the rotation.
                //transform.rotation = smoothedRotation; // Also works.
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    smoothedRotation,
                    Time.fixedDeltaTime * 100f);
            }
            // TRANSLATION -----------------------------------------------------

            if (PlayerCombat.isLockingOn)
            {
                animator.SetBool(IsStrafing, true);

                var lockOnDirection = (playerLockOnCamera.transform.forward * verticalInput +
                                       playerLockOnCamera.transform.right * horizontalInput).normalized;

                var strafeSpeed = movementSpeed * strafeMovementSpeedModifier;

                var inputMagnitude = Mathf.Sqrt(horizontalInput * horizontalInput + verticalInput * verticalInput);
                var newForce = new Vector3(
                    lockOnDirection.x * strafeSpeed * horizontalMultiplier * inputMagnitude, downpullForce,
                    lockOnDirection.z * strafeSpeed * horizontalMultiplier * inputMagnitude);


                var currentVelocity = rigidBody.velocity;
                rigidBody.AddForce(newForce - new Vector3(currentVelocity.x, 0, currentVelocity.z),
                    ForceMode.VelocityChange);
            } else {
                animator.SetBool(IsStrafing, false);
                Vector3 newRootPosition = animator.rootPosition;

                // Smooth the translation.
                newRootPosition = Vector3.LerpUnclamped(
                    transform.position,
                    newRootPosition,
                    movementSpeed
                );

                // Apply the translastion.
                rigidBody.MovePosition(newRootPosition);
            }
        }
    }

    private void Update()
    {
        AnimatorPickupLogic();
    }

    void AnimatorPickupLogic()
    {
        if (!tryPickup)
            return;

        var animState = animator.GetCurrentAnimatorStateInfo(0);

        if (!fruitDetector || !fruitDetector.FruitNearby() || !animState.IsName("MovementTree") || !fruitDetector.FruitNearby())
        {
            tryPickup = false;
            return;
        }

        Debug.Log("Trying pickup!");

        animator.SetTrigger("DoPickup");
        tryPickup = false;
        targetFruitForPickup = fruitDetector.GetClosestFruit();
    }


    public bool ControlsEnabled => controls.Gameplay.enabled;

    /// <summary>
    /// External API to update control state
    /// </summary>
    /// <param name="isEnabled"></param>
    public void UpdateControlState(bool enableControlState)
    {
        if (enableControlState)
        {
            controls.Gameplay.Enable();
        }
        else
        {
            controls.Gameplay.Disable();
        }
    }

    /// <summary>
    /// External API to halt animation motion
    /// TODO: replace with some kind of graceful slowdown eventually
    /// </summary>
    public void StopAnimMotion()
    {
        //Debug.Log("- StopAnimMotion -");
        animator.SetFloat("Speed", 0f);
        animator.SetBool("IsFalling", false);
        animator.SetBool("IsGrounded", true);
        animator.Play("MovementTree");
        horizontalInput = 0;
        verticalInput = 0;
        isGrounded = true;
        isFalling = false;
        fallingTimeout = 5f;
        applyInitialFallMomentum = true;
    }



    private void OnAnimatorIK(int layerIndex)
    {
        if (!animator)
            return;

        AnimatorStateInfo astate = animator.GetCurrentAnimatorStateInfo(0);
        if (astate.IsName("Picking Up") && targetFruitForPickup)
        {
            float fruitContactWeight = animator.GetFloat("fruitClose");


            Vector3 targetPosition = targetFruitForPickup.GetComponent<SphereCollider>().ClosestPoint(touchToPickup.position);

            if (targetFruitForPickup != null)
            {
                animator.SetLookAtWeight(fruitContactWeight);
                animator.SetLookAtPosition(targetPosition);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, fruitContactWeight);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPosition);

                if (fruitContactWeight > 0.95f)
                {
                    targetFruitForPickup.SetParent(leftHand);
                    targetFruitForPickup.position = Vector3.Lerp(leftHand.position, targetFruitForPickup.position, 0.01f);
                    targetFruitForPickup.GetComponent<Rigidbody>().isKinematic = true;
                    targetFruitForPickup.GetComponent<BoxCollider>().enabled = false;

                    // we have it now, it's not a nearby fruit anymore
                    fruitInHand = targetFruitForPickup;
                    fruitDetector.RemoveFruit(fruitInHand);
                }
            }
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetLookAtWeight(0);
        }
    }


    protected void EatFruit()
    {
        fruitInHand.GetComponent<HealingApple>().BeConsumed(transform);
        fruitInHand = null;
    }

    void EmitFootstep1FromAnim()
    {
        //EventManager.TriggerEvent<PlayerFootstepEvent, Vector3, int>(transform.position, 0);
    }

    void EmitFootstep2FromAnim()
    {
        //EventManager.TriggerEvent<PlayerFootstepEvent, Vector3, int>(transform.position, 1);
    }

    public void CheckFootSounds()
    {
        if (animator == null)
        {
            return;
        }

        RaycastHit hit;
        Ray ray;

        ray = new Ray(leftFoot.position + Vector3.up, Vector3.down);
        bool leftFootDidHit = Physics.Raycast(ray, out hit, maxDistanceToGroundForFootstep + 1f, whatIsGround);

        // if left foot was down, but we no longer hit, left foot is up!
        if (leftFootDown && !leftFootDidHit)
        {
            leftFootDown = false;
        }
        else if (!leftFootDown && leftFootDidHit)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                Vector3 hitPosition = hit.point;

                EventManager.TriggerEvent<PlayerFootstepEvent, Vector3, int>(hitPosition, 0);
                leftFootDown = true;
            }

        }

        ray = new Ray(rightFoot.position + Vector3.up, Vector3.down);
        bool rightFootDidHit = Physics.Raycast(ray, out hit, maxDistanceToGroundForFootstep + 1f, whatIsGround);

        // if left foot was down, but we no longer hit, left foot is up!
        if (rightFootDown && !rightFootDidHit)
        {
            rightFootDown = false;
        }
        else if (!rightFootDown && rightFootDidHit)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                Vector3 hitPosition = hit.point;

                EventManager.TriggerEvent<PlayerFootstepEvent, Vector3, int>(hitPosition, 1);
                rightFootDown = true;
            }

        }



    }
}
