using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class WorldSpaceMovement : MonoBehaviour, WorldSpaceControls.IWorldSpaceActionMapActions
{
    [SerializeField] private WorldSpaceControls worldSpaceControls;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask whatIsGround;
        
    public CharacterController controller;
    public Transform camera;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    
    private bool useAnimator;
    private Collider collider;
    private Vector2 move;

    private float distanceToGround;
    
    private static readonly int AnimatorMovementParameter = Animator.StringToHash("Input");
    private static readonly int AnimatorIsMovingParameter = Animator.StringToHash("IsMoving");

    private void Awake()
    {
        if (animator != null)
        {
            Debug.Log("Using root motion");
            useAnimator = true;
        }

        collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.Log("WorldSpaceMovement could not find collider");
        }

        worldSpaceControls = new WorldSpaceControls();
        worldSpaceControls.WorldSpaceActionMap.SetCallbacks(this);
    }

    private void Start()
    {
        if (collider != null)
        {
            distanceToGround = collider.bounds.extents.y;
        }
    }

    private void Update()
    {
        // Better root detection
        if (useAnimator) MoveCharacterControllerRoot();
        MoveCharacterController();
    }

    private void OnAnimatorMove()
    {
        Vector3 velocity = animator.deltaPosition;
        velocity.y = Physics.gravity.y * Time.deltaTime;

        controller.Move(velocity);
    }

    private void MoveCharacterControllerRoot()
    {
        var inputMagnitude = Mathf.Clamp01(move.magnitude);

        if (move == Vector2.zero || !controller.isGrounded)
        {
            animator.SetBool(AnimatorIsMovingParameter, false);
            animator.SetFloat(AnimatorMovementParameter, 0f, 0.05f, Time.deltaTime);
            
            // Fix this to move with camera
            Vector3 gravity = new Vector3(0f, Physics.gravity.y, 0f);
            controller.Move(gravity * Time.deltaTime);
            return;
        }

        animator.SetFloat(AnimatorMovementParameter, inputMagnitude, 0.05f, Time.deltaTime);
        animator.SetBool(AnimatorIsMovingParameter, true);
        var movement = new Vector3(move.x, 0, move.y);

        /* Make Relative to Camera */
        var targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        
        // Look the right way
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
    
    private void MoveCharacterController()
    {
        var inputMagnitude = Mathf.Clamp01(move.magnitude);
        animator.SetFloat(AnimatorMovementParameter, inputMagnitude, 0.05f, Time.deltaTime);
        
        move.Normalize();
        
        if (move == Vector2.zero) // || !CheckGroundNear(transform.position))
        {
            Vector3 down = Vector3.down * 9.8f;
            controller.Move(down * Time.deltaTime);
            return;
        }

        var movement = new Vector3(move.x, 0, move.y);

        /* Make Relative to Camera */
        float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        
        // Look the right way
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        
        // Move the right way
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        Vector3 player = moveDir.normalized * speed;
        
        /*
         * Scaling gravity so player falls faster
         * Current IsGrounded 
         */
        player.y = Physics.gravity.y;
        
        controller.Move(player *  Time.deltaTime);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnMoveCamera(InputAction.CallbackContext context)
    {
        return;
    }

    private void OnEnable()
    {
        worldSpaceControls.Enable();
    }

    private void OnDisable()
    {
        worldSpaceControls.Disable();
    }
}
