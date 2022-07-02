using System.Collections;
using TMPro;
using UnityEngine;

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
    public float rotationSmoothTime = 0.1f;
    float rotationVelocity = 0f;

    // Used for slope handling and falling off cliffs.
    TerrainData terrainData;
    Vector3 terrainSize;
    float downpullForce = 0f;
    float horizontalMultiplier = 1f;
    bool hasLanded = true;

    // Character movement will be relative to this camera.
    private Transform cam;

    // Action related stuff
    private float rotationAngle;

    // Jumping
    private bool isJumping;
    private bool isGrounded;

    // Attack & Defense
    private bool doBlock;
    private bool doSlash;
    private Sword sword;
    private int slashLungeFrameCtr = 0;
    public int slashLungeFrameLen = 15;
    public float slashLungeTransformMultiplier = 3.5f;
    private bool hasSkill;

    // Components of the player GameObject we reference
    private Rigidbody rigidBody;
    private Animator animator;

    [SerializeField]
    private TextMeshProUGUI GameMessage = null;

    // Helpers
    private bool ShouldLunge => sword.InUse && slashLungeFrameCtr < slashLungeFrameLen && verticalInput > .1;
    private bool IsAnimStateSwordSlash => animator.GetCurrentAnimatorStateInfo(0).IsName("SwordSlash");
    private bool IsAnimStateSwordBackSlash => animator.GetCurrentAnimatorStateInfo(0).IsName("SwordBackSlash");
    private bool IsAnimStateJumpSlash => animator.GetCurrentAnimatorStateInfo(0).IsName("JumpSlash");

    // Stuff to help us know when we're in contact with the ground
    [SerializeField] 
    private LayerMask whatIsGround;

    public float playerSpeedForce;

    CharacterController characterController;
    CapsuleCollider capsuleCollider;

    private void Awake()
    {
        GetTerrainInfo();
        GetCamera();
        GatherComponents();
        SetPlayerPhysicalProperties();
        SetupControls();
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
        cam = GameObject.Find("CameraParent/MainCamera").transform;
        Utility.LogErrorIfNull(cam, "Player Main Camera");
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

        controls.Gameplay.Slash.performed += ctx =>
        {
            doSlash = true;

            // This is a horrible place for this.
            // Need to figure out a way to make it so this is only reset once per slash animation
            slashLungeFrameCtr = 0;
        };

        controls.Gameplay.Shield.performed += ctx =>
        {
            doBlock = true;
        };

        controls.Gameplay.Shield.canceled += ctx =>
        {
            doBlock = false;
        };

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

    // Start is called before the first frame update
    void Start()
    {
        isGrounded = true;
        isJumping = false;
        doSlash = false;
        doBlock = false;
    }


    void GatherComponents()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rigidBody = GetComponent<Rigidbody>();
        sword = GameObject.Find("Sword").GetComponent<Sword>();
    }


    void ApplyTransforms()
    {
        var transformForward = transform.forward;

        transformForward = ShouldLunge ?
            transform.forward * slashLungeTransformMultiplier :
            transformForward;

        // The final jump animation should lunge differently, not sure how
        transformForward = !ShouldLunge && (IsAnimStateSwordSlash || IsAnimStateSwordBackSlash) ?
            transform.forward * 0 :
            transformForward;

        if (ShouldLunge)
        {
            rigidBody.AddForce(transformForward, ForceMode.Impulse);
            slashLungeFrameCtr++;
        }

        if (isGrounded)
        {
            rigidBody.drag = 5;

            if (!doBlock)
            {
                rigidBody.AddForce(transformForward * verticalInput * playerSpeedForce, ForceMode.Force);
            }
        }
        else
        {
            rigidBody.drag = 0;
        }
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

        /*
        if (Physics.Raycast(groundInterceptRayStart, Vector3.down, groundInterceptRayLength, whatIsGround))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        */

        if (isJumping && isGrounded)
        {
            rigidBody.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
            isJumping = false;
        }
    }

    // Called before Update(). All physics calculations occur immediately after.
    private void FixedUpdate()
    {
        ApplyForcesAndDrag();
        ApplyCharacterMovement();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyTransforms();
        UpdateAnimator();
    }

    private void ApplyCharacterMovement()
    {
        // The player movement is relative to the position of the camera:
        //
        //  - Forward   : Player moves in the direction the camera is facing
        //                (i.e. away from the camera).
        //  - Backward  : Player moves towards the camera.
        //  - Left      : Player moves towards the camera's left.
        //  - Right     : Player moves towards the camera's right.
        //
        //  - The player cannot move whilst attacking/blocking, but can rotate
        //    to reorient itself towards the target.

        /// \todo A lock-on system might be a feature to consider in the future.
        /// Define a priority for this.

        if (movementMagnitude >= 0.1)
        {
            // ROTATION --------------------------------------------------------

            // Compute how much the player needs to rotate around its own
            // Y/Vertical axis to align itself with the desired orientation.
            float targetAngle = (Mathf.Rad2Deg *
                    Mathf.Atan2(playerOrientation.x, playerOrientation.z));
            // Make the rotation relative to the camera's Y/Vertical position.
            targetAngle += cam.eulerAngles.y;

            // Smooth the angle transition.
            float smoothedAngle = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y, // Original player orientation.
                    targetAngle,             // Desired player orientation.
                    ref rotationVelocity,
                    rotationSmoothTime);

            // Compute the player's angular and linear movements:
            // Rotation: Around its own Y/Vertical axis.
            Quaternion sharpRotation = Quaternion.Euler(0f, targetAngle, 0f);
            Quaternion smoothedRotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            // Apply the rotation.
            //transform.rotation = smoothedRotation; // Also works.
            transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    smoothedRotation,
                    Time.fixedDeltaTime * 100f);

            // TRANSLATION -----------------------------------------------------

            if ((!doBlock) && (!sword.InUse))
            {
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
                Debug.Log("groundNormal: " + groundNormal);
                Debug.Log("slopeAngle: " + slopeAngle);

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

                Vector3 newDirection = new Vector3(
                        transform.forward.x * horizontalMultiplier,
                        downpullForce,
                        transform.forward.z * horizontalMultiplier);

                // Apply the translastion.
                // Note: Subtract the current velocity to get constant
                // velocity (i.e. no acceleration).
                rigidBody.AddForce(
                    (newDirection * movementSpeed) - rigidBody.velocity,
                    ForceMode.VelocityChange);
            }
        }
        else
        {
            // Stopping the character model in its tracks avoids the slippery
            // Luigi-like movement. Makes the character movement more
            // responsive.
            rigidBody.velocity = new Vector3(0f, rigidBody.velocity.y, 0f);
        }
    }

    private void UpdateAnimator()
    {
        AnimatorBlockLogic();
        AnimatorSlashLogic();
    }

    void AnimatorBlockLogic()
    {
        var animState = animator.GetCurrentAnimatorStateInfo(0);

        // give block priority - get rid of doSlash
        if (doBlock)
        {
            animator.SetBool("DoBlock", true);
            animator.SetBool("DoAttack", false);
            animator.SetBool("DoBackslash", false);
            animator.SetBool("DoJumpSlash", false);

            if (animState.IsName("Block"))
            {
                // set this here because we may have circumvented
                // the normal way around this.

                sword.InUse = false;
            }

        }
        else
        {
            animator.SetBool("DoBlock", false);
        }
    }

    void AnimatorSlashLogic()
    {
        var animState = animator.GetCurrentAnimatorStateInfo(0);

        if (doSlash && !doBlock)
        {
            doSlash = false;

            if (animState.IsName("MovementTree"))
            {
                sword.InUse = true;
                animator.SetBool("DoAttack", true);
                //EventManager.TriggerEvent<SwordSwingEvent, Vector3, float>(transform.position, 0);
            }
            else if (IsAnimStateSwordSlash)
            {

                sword.InUse = true;
                animator.SetBool("DoBackslash", true);
            }
            else if (IsAnimStateSwordBackSlash)
            {
                sword.InUse = true;
                animator.SetBool("DoJumpSlash", true);
            }

        }
        else
        {
            if (animState.IsName("MovementTree"))
            {
                sword.InUse = false;
            }
            else if (IsAnimStateSwordSlash)
            {
                // clear this so it can be picked up later
                animator.SetBool("DoAttack", false);
                sword.InUse = true;
            }
            else if (IsAnimStateSwordBackSlash)
            {
                animator.SetBool("DoBackslash", false);
                sword.InUse = true;
            }
            else if (IsAnimStateJumpSlash)
            {
                animator.SetBool("DoJumpSlash", false);
                sword.InUse = true;
            }
        }
    }

    private void ShowMessage(string message, uint timeout = 3)
    {
        GameMessage.SetText(message);

        if (timeout > 0)
        {
            StartCoroutine(DestroyMessage(timeout));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AbilityPickup"))
        {
            ShowMessage("You acquired a new ability!");
            other.gameObject.SetActive(false);
        }
    }
    private IEnumerator DestroyMessage(float waitTime)
    {
        Debug.Log($"Destroying message in {waitTime.ToString()} seconds...");
        yield return new WaitForSeconds(waitTime);

        Debug.Log("Destroying message");
        GameMessage.SetText("");
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

    /*
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Ontriggerexit");
        
    }
    */
}
