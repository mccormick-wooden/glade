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

    public CharacterController controller;
    public Transform camera;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private Vector2 move;

    private void Awake()
    {
        
        worldSpaceControls = new WorldSpaceControls();
        worldSpaceControls.WorldSpaceActionMap.SetCallbacks(this);
    }

    private void Update()
    {
        MoveCharacterController();
    }

    private void MoveCharacterController()
    {
        if (move == Vector2.zero)
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
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        
        // Move the right way
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        Vector3 player = moveDir.normalized * speed;
        player.y = -9.8f;
        
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
