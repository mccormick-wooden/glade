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

        EventManager.TriggerEvent<PlayMusicEvent, int>(0);

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
        if (!doBlock)
        {
            transform.Rotate(0, horizontalInput, 0);
        }

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
