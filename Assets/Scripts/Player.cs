using System;
using System.Collections;
using PlayerBehaviors;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;

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
    private Vector3 playerOrientation => new Vector3(horizontalInput, 0f, verticalInput).normalized;
    private float movementMagnitude => playerOrientation.magnitude;

    // Controls how fast the character moves and turns.
    public float movementSpeed = 10f;
    public float strafeMovementSpeedModifier = 0.6f;
    public float rotationSmoothTime = 0.1f;
    float rotationVelocity = 0f;

    // Used for slope handling and falling off cliffs.
    TerrainData terrainData;
    Vector3 terrainSize;
    float downpullForce = 0f;
    float horizontalMultiplier = 1f;
    bool hasLanded = true;

    // Character movement will be relative to this camera.
    public PlayerLockOnCamera playerLockOnCamera;
    private Transform mainCameraTransform;

    // Action related stuff
    private float rotationAngle;

    // Jumping
    private bool isJumping;
    private bool isGrounded;

    // Components of the player GameObject we reference
    private Rigidbody rigidBody;
    private Animator animator;

    // Helpers
    private bool IsAnimStateSwordSlash => animator.GetCurrentAnimatorStateInfo(0).IsName("SwordSlash");
    private bool IsAnimStateSwordBackSlash => animator.GetCurrentAnimatorStateInfo(0).IsName("SwordBackSlash");
    private bool IsAnimStateJumpSlash => animator.GetCurrentAnimatorStateInfo(0).IsName("JumpSlash");

    // Stuff to help us know when we're in contact with the ground
    [SerializeField] private LayerMask whatIsGround;

    public float playerSpeedForce;

    CharacterController characterController;
    CapsuleCollider capsuleCollider;

    #region CombatProperties

    private PlayerCombat playerCombat;
    private PlayerWeaponManager playerWeaponManager;
    [SerializeField] private Weapon primaryWeapon;
    [SerializeField] private Weapon secondaryWeapon;
    private static readonly int HorizontalInput = Animator.StringToHash("horizontalInput");
    private static readonly int VerticalInput = Animator.StringToHash("verticalInput");
    private static readonly int IsStrafing = Animator.StringToHash("isStrafing");

    #endregion

    private void Awake()
    {
        # region CombatAwake

        playerCombat = GetComponent<PlayerCombat>();
        Utility.LogErrorIfNull(playerCombat, "playerCombat",
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

        isGrounded = true;
        isJumping = false;
    }

    private void FixedUpdate()
    {
        ApplyForcesAndDrag();
        ApplyCharacterMovement();

        animator.SetFloat(HorizontalInput, horizontalInput);
        animator.SetFloat(VerticalInput, verticalInput);
    }
    
    private void GetTerrainInfo()
    {
        /// \todo Find a way to dynamically load the terrain data of the active
        /// scene.
        terrainData = (TerrainData)Resources.Load("Assets/Terrain/Terrain_0_0_1e9bf6a0-0e4a-41f6-9cb7-c6586c914a9a");
        terrainSize = Terrain.activeTerrain.terrainData.size;
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

            // Ignore any negligible motion.
            if (movementMagnitude < 0.1)
            {
                horizontalInput = 0f;
                verticalInput = 0f;
            }

            // Update animation.
            animator.SetFloat("Speed", movementMagnitude);
        };
    }

    private void SetupAttackControls()
    {
        controls.Gameplay.LockOnToggle.performed += ctx => playerCombat.HandleLockOnInput();
        controls.Gameplay.Slash.performed += ctx => playerCombat.PerformSlashAttack(primaryWeapon);
        controls.Gameplay.SpecialAttack.performed += ctx => playerCombat.PerformHeavyAttack(primaryWeapon);
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
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

    void ApplyForcesAndDrag()
    {
        float halfHeight = capsuleCollider.height / 2f;
        Vector3 groundInterceptRayStart = transform.position + new Vector3(0, halfHeight, 0);
        float groundInterceptRayLength = halfHeight + 0.25f;

        // just a dummy thing for now because we don't have jump

        bool isJumpable = false;
        isGrounded = CheckGroundNear(transform.position, out isJumpable);

        if (isJumping && isGrounded)
        {
            rigidBody.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
            isJumping = false;
        }
    }

    private void ApplyCharacterMovement()
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

       
        if (movementMagnitude >= 0.1)
        {
            if (playerCombat.isLockingOn)
            {
                // Look at the target
                var lockOnTarget = playerCombat.currentLockedOnTarget.transform.position;
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

            // Determine whether the player is going uphill or downhill:
            // 1. Get the player's position on the map (horizontal plane).
            var normalizedXposition = transform.position.x / terrainSize.x;
            var normalizedZposition = transform.position.z / terrainSize.z;
            // 2. Get the terrain's Normal on that point.
            var groundNormal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(
                normalizedXposition, normalizedZposition);
            // 3. Get the angular difference between the terrain's Normal
            // and the player's forward component.
            // Note: Use an offset of -90 degrees to make a perfect
            // alignment equal to 0.
            // Uphill: positive angles.
            // Downhill: negative angles.
            var slopeAngle = Vector3.Angle(groundNormal, transform.forward) - 90f;
            //Debug.Log("groundNormal: " + groundNormal);
            //Debug.Log("slopeAngle: " + slopeAngle);

            if (hasLanded)
            {
                // Apply an appropriate downpull force in order to handle
                // downhill slopes (and avoid keep walking on air when
                // jumping off cliffs).
                if (slopeAngle > -5f)
                {
                    // Mild downhill slopes.
                    downpullForce = 0f;
                    horizontalMultiplier = 1f;
                }
                else if (slopeAngle > -30f)
                {
                    // Pronounced downhill slopes.
                    downpullForce = -0.5f;
                    horizontalMultiplier = 1f;
                }
                else
                {
                    // Very steep downhill slopes/cliffs.
                    downpullForce = -0.8f;
                    horizontalMultiplier = 0.8f;
                    hasLanded = false;
                }
            }

            // When locking on, we face the lock on target and strafe
            if (playerCombat.isLockingOn)
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
            }
            else
            {
                var newDirection = new Vector3(
                    transform.forward.x * horizontalMultiplier,
                    downpullForce,
                    transform.forward.z * horizontalMultiplier);

                // Apply the translastion.
                // Note: Subtract the current velocity to get constant
                // velocity (i.e. no acceleration).
                rigidBody.AddForce(
                    newDirection * movementSpeed - rigidBody.velocity,
                    ForceMode.VelocityChange);
            }
        }
        else
        {
            animator.SetBool(IsStrafing, false);
        }
    }
    public void EmitFirstSwordSlash()
    {
        EventManager.TriggerEvent<SwordSwingEvent, Vector3, int>(transform.position, 0);
    }

    public void EmitSecondSwordSlash()
    {
        EventManager.TriggerEvent<SwordSwingEvent, Vector3, int>(transform.position, 1);
    }

    public void EmitThirdSwordSlash()
    {
        EventManager.TriggerEvent<SwordSwingEvent, Vector3, int>(transform.position, 2);
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
        animator.SetFloat("Speed", 0f);
        horizontalInput = 0;
        verticalInput = 0;
    }
}
