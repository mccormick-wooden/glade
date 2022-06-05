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

    private int swordSlashState = 0;
    private int numberOfFramesSinceLastSwing = 0;

    // Components of the player GameObject we reference
    private Rigidbody rigidBody;
    private Animator animator;

    // Camera related stuff
    //   - capture camera inputs here because we already have the controls
    private Transform objectToLookAt;

    // Helpers
    private bool ShouldLunge => sword.IsSwinging && slashLungeFrameCtr < slashLungeFrameLen;
    private bool IsAnimStateSwordSlash => animator.GetCurrentAnimatorStateInfo(0).IsName("SwordSlash");
    private bool IsAnimStateSwordBackSlash => animator.GetCurrentAnimatorStateInfo(0).IsName("SwordBackSlash");
    private bool IsAnimStateJumpSlash => animator.GetCurrentAnimatorStateInfo(0).IsName("JumpSlash");


    [SerializeField] 
    private Transform groundCheckTransform = null;

    [SerializeField] 
    private LayerMask foo;

    /*Vector2 move;
    Vector2 rotate;*/

    [SerializeField]
    private GameObject prefab;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sword = GameObject.Find("Sword").GetComponent<Sword>();

        controls = new CharacterPlayerControls();

        controls.Gameplay.Move.performed += ctx => 
            {
                Vector2 leftStick = ctx.ReadValue<Vector2>();
                //horizontalInput = move.x/4f; 
                //horizontalInput = move.x;
                verticalInput = leftStick.y;
                if (verticalInput < 0.1 && verticalInput > -0.1) verticalInput = 0;

                animator.SetFloat("Speed", verticalInput);
            };

        controls.Gameplay.Rotate.performed += ctx => 
            { 
                Vector2 rightStick = ctx.ReadValue<Vector2>();
                horizontalInput = rightStick.x;
                if (horizontalInput < 0.1 && horizontalInput > -0.1) horizontalInput = 0;

                //animator.SetFloat("TurningSpeed", horizontalInput);
                //cameraHorizontalAngleChange = look.x;
                //cameraVerticalAngleChange = look.y;
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
        rigidBody = GetComponent<Rigidbody>();

        isGrounded = true;
        isJumping = false;
        doSlash = false;
        swordSlashState = 0;
        doBlock = false;
       
        CameraHorizontalAngleChange = 0f;
        CameraVerticalAngleChange = 0f;
    }

    void CalculateCameraPosition()
    {
        // first, rotate the player model in place, without any translation
        //transform.Rotate(0, horizontalInput/4, 0);

        rotationAngle = Vector3.SignedAngle(new Vector3(0, 0, 1), transform.forward, new Vector3(0,1,0));
    }

    void UpdateAnimations()
    {
        
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

        if (ShouldLunge) slashLungeFrameCtr++;

        rigidBody.MovePosition(rigidBody.position + transformForward * verticalInput * Time.deltaTime * 10);
        rigidBody.MoveRotation(rigidBody.rotation * Quaternion.AngleAxis(horizontalInput * Time.deltaTime * 100, Vector3.up));
        
        //animator.SetFloat("TurningSpeed", horizontalInput);
        //animator.SetFloat("Speed", verticalInput);
        //anim.SetBool("isFalling", !isGrounded);
    }

    void DoPhysicsChecks()
    {
        /*if (Physics.OverlapBox(groundCheckTransform.position, new Vector3(0.01f, 0.01f, 0.01f)).Length == 0)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = true;
        }*/
    }

    void CheckRegionForCameraOverrides()
    {
        Ray ray = new Ray(transform.position, new Vector3(0, -1, 0));
        Debug.DrawRay(transform.position, Vector3.down, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "LookUp")
            {
                objectToLookAt = GameObject.Find("LookatUpPosition").transform;
            }
            else
            {
                objectToLookAt = transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ApplyTransforms();
        UpdateAnimations();
        DoPhysicsChecks();
        CalculateCameraPosition();
    }

    /*
    private void OnAnimatorMove()
    {
        Vector3 newRootPosition;
        Quaternion newRootRotation;

        //newRootRotation = animator.rootRotation;
        newRootPosition = animator.rootPosition;

        rigidBody.MovePosition(newRootPosition);
        //rigidBody.MoveRotation(newRootRotation);
    }
    */

    private void FixedUpdate()
    {
        if (isJumping && isGrounded)
        {
            rigidBody.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
            isJumping = false;
        }

        //float veritcalVelocity = rigidBody.velocity.y;
        //rigidBody.velocity = transform.forward * verticalInput * 4f + new Vector3(0, veritcalVelocity, 0);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2 + Camera.main.pixelHeight / 10, 0));


        /*if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            GameObject cube = GameObject.Find("AntiGravityCubePlanning");
            cube.transform.position = hit.point;
        };*/

        //var animState = animator.GetCurrentAnimatorStateInfo(0);

        /*
        if (!animState.IsName("SwordSlash"))
        {
            animator.SetBool("DoAttack", doSlash);

            if (doSlash)
            {
                sword.isSwinging = true;
            }
            else
            {
                sword.isSwinging = false;
            }

            doSlash = false;
        }
        else
        {
            animator.SetBool("DoAttack", false);
            sword.isSwinging = true;
        }*/


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

        if (doSlash && !doBlock)
        {
            doSlash = false;

            if (animState.IsName("MovementTree"))
            {
                numberOfFramesSinceLastSwing = 0;
                //swordSlashState = 1;
                sword.IsSwinging = true;
                animator.SetBool("DoAttack", true);
            }
            else if (IsAnimStateSwordSlash)
            {
                numberOfFramesSinceLastSwing = 0;
                //swordSlashState = 2;
                sword.IsSwinging = true;
                animator.SetBool("DoBackslash", true);
            }
            else if (IsAnimStateSwordBackSlash)
            {
                numberOfFramesSinceLastSwing = 0;
                //swordSlashState = 2;
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

    /*
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ontriggerenter");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Ontriggerexit");
        
    }
    */
}
