using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Controls related stuff
    CharacterPlayerControls controls;
    private float verticalInput;
    private float horizontalInput;

    // Action related stuff
    private bool jumping;
    private bool isGrounded;
    private float rotationAngle;

    // Components of the player GameObject we reference
    Rigidbody rigidBody;
    Animator animator;


    // Camera related stuff
    //   - capture camera inputs here because we already have the controls
    Transform objectToLookAt;
    public float cameraHorizontalAngleChange { get; private set; }
    public float cameraVerticalAngleChange { get; private set; }
 


    public Transform playerTransform { get { return transform; } }
    public float RotationAngle { get { return rotationAngle; } }

    [SerializeField] private Transform groundCheckTransform = null;
    [SerializeField] private LayerMask foo;

    /*Vector2 move;
    Vector2 rotate;*/

    bool doSlash;
    int swordSlashState = 0;
    Sword sword;

    bool doBlock;

    int numberOfFramesSinceLastSwing = 0;

    [SerializeField] private GameObject prefab;

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
        jumping = false;
        doSlash = false;
        swordSlashState = 0;
        doBlock = false;
       
        cameraHorizontalAngleChange = 0f;
        cameraVerticalAngleChange = 0f;
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


    void CheckInputs()
    {
        rigidBody.MovePosition(rigidBody.position + this.transform.forward * verticalInput * Time.deltaTime * 10);
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
        CheckInputs();
        UpdateAnimations();
        DoPhysicsChecks();
        CalculateCameraPosition();
    }

    
    /*
     * private void OnAnimatorMove()
    {
        Vector3 newRootPosition;
        Quaternion newRootRotation;

        newRootRotation = animator.rootRotation;
        newRootPosition = animator.rootPosition;

        rigidBody.MovePosition(newRootPosition);
        //rigidBody.MoveRotation(newRootRotation);
    }
    */
    

    private void FixedUpdate()
    {
        if (jumping && isGrounded == true)
        {
            rigidBody.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
            jumping = false;
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
                sword.isSwinging = false;
            }

        }
        else if (doSlash)
        {
            doSlash = false;

            if (animState.IsName("MovementTree"))
            {
                numberOfFramesSinceLastSwing = 0;
                //swordSlashState = 1;
                sword.isSwinging = true;
                animator.SetBool("DoAttack", true);
            }
            else if (animState.IsName("SwordSlash"))
            {
                numberOfFramesSinceLastSwing = 0;
                //swordSlashState = 2;
                sword.isSwinging = true;
                animator.SetBool("DoBackslash", true);
            }
            else if (animState.IsName("SwordBackSlash"))
            {
                numberOfFramesSinceLastSwing = 0;
                //swordSlashState = 2;
                sword.isSwinging = true;
                animator.SetBool("DoJumpSlash", true);
            }

        }
        else
        {
            if (animState.IsName("MovementTree"))
            {
                sword.isSwinging = false;
            }
            // || animState.IsName("SwordSlash") || animState.IsName("SwordBackSlash"))
            else if (animState.IsName("SwordSlash"))
            {
                // clear this so it can be picked up later
                animator.SetBool("DoAttack", false);
                sword.isSwinging = true;
            }
            else if (animState.IsName("SwordBackSlash"))
            {
                animator.SetBool("DoBackslash", false);
                sword.isSwinging = true;
            }
            else if (animState.IsName("JumpSlash"))
            {
                animator.SetBool("DoJumpSlash", false);
                sword.isSwinging = true;
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
