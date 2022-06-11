using UnityEngine;

public class Player : MonoBehaviour
{
    
    public Transform PlayerTransform => transform;

    // Controls related stuff
    private CharacterPlayerControls controls;
    private float verticalInput;
    private float horizontalInput;

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

    // Components of the player GameObject we reference
    private Rigidbody rigidBody;
    private Animator animator;



    // Helpers
    private bool ShouldLunge => sword.IsSwinging && slashLungeFrameCtr < slashLungeFrameLen;
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
        GatherComponents();
        SetPlayerPhysicalProperties();
        SetupControls();
    }

    void SetPlayerPhysicalProperties()
    {
        rigidBody.mass = 85;
        rigidBody.drag = 5;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        playerSpeedForce = 1250;
    }


    void SetupControls()
    {
        controls = new CharacterPlayerControls();

        controls.Gameplay.Move.performed += ctx =>
        {
            Vector2 leftStick = ctx.ReadValue<Vector2>();
            verticalInput = leftStick.y;
            if (verticalInput < 0.1 && verticalInput > -0.1) verticalInput = 0;

            animator.SetFloat("Speed", verticalInput);
        };

        controls.Gameplay.Rotate.performed += ctx =>
        {
            Vector2 rightStick = ctx.ReadValue<Vector2>();
            horizontalInput = rightStick.x;
            if (horizontalInput < 0.1 && horizontalInput > -0.1) horizontalInput = 0;
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
        transform.Rotate(0, horizontalInput, 0);

        var transformForward = transform.forward;

        transformForward = ShouldLunge ? 
            transform.forward * slashLungeTransformMultiplier : 
            transformForward;
                                            // The final jump animation should lunge differently, not sure how
        transformForward = !ShouldLunge && (IsAnimStateSwordSlash || IsAnimStateSwordBackSlash) ?
            transform.forward * 0 :
            transformForward;

        if (ShouldLunge) slashLungeFrameCtr++;    
        
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

    void ApplyForcesAndDrag()
    {
        float halfHeight = capsuleCollider.height / 2f;
        Vector3 groundInterceptRayStart = transform.position + new Vector3(0, halfHeight, 0);
        float groundInterceptRayLength = halfHeight + 0.1f;

        if (Physics.Raycast(groundInterceptRayStart, Vector3.down, groundInterceptRayLength, whatIsGround))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isJumping && isGrounded)
        {
            rigidBody.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
            isJumping = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ApplyTransforms();
        UpdateAnimator();
    }


    private void FixedUpdate()
    {
        ApplyForcesAndDrag();
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
                sword.IsSwinging = false;
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
                sword.IsSwinging = true;
                animator.SetBool("DoAttack", true);
            }
            else if (IsAnimStateSwordSlash)
            {
                sword.IsSwinging = true;
                animator.SetBool("DoBackslash", true);
            }
            else if (IsAnimStateSwordBackSlash)
            {
                sword.IsSwinging = true;
                animator.SetBool("DoJumpSlash", true);
            }

        }
        else
        {
            if (animState.IsName("MovementTree"))
            {
                sword.IsSwinging = false;
            }
            else if (IsAnimStateSwordSlash)
            {
                // clear this so it can be picked up later
                animator.SetBool("DoAttack", false);
                sword.IsSwinging = true;
            }
            else if (IsAnimStateSwordBackSlash)
            {
                animator.SetBool("DoBackslash", false);
                sword.IsSwinging = true;
            }
            else if (IsAnimStateJumpSlash)
            {
                animator.SetBool("DoJumpSlash", false);
                sword.IsSwinging = true;
            }
        }
    }
}
