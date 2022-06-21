using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Movement
{
    public class CameraRelativeRootMovement : MonoBehaviour, CameraRelativeControls.ICameraRelativeActionMapActions
    {
        [SerializeField] private Animator animator;
     
        private CameraRelativeControls _controls;
        
        public CharacterController controller;
        public new Transform camera;

        public float speed = 6f;
        public float turnSmoothTime = 0.1f;
        private float _turnSmoothVelocity;
        
        [SerializeField] private Vector2 move;

        private static readonly int AnimatorMovementParameter = Animator.StringToHash("Input");
        private static readonly int AnimatorIsMovingParameter = Animator.StringToHash("IsMoving");

        private void Awake()
        {
            if (animator == null) Debug.LogError("Could not get Animator component");
            if (controller == null) Debug.LogError("Could not get CharacterController component");

            _controls = new CameraRelativeControls();
            _controls.CameraRelativeActionMap.SetCallbacks(this);
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {

            var inputMagnitude = Mathf.Clamp01(move.magnitude);

            if (inputMagnitude < 0.001 || !controller.isGrounded)
            {
                animator.SetFloat(AnimatorMovementParameter, 0f, 0.1f, Time.deltaTime);
                animator.SetBool(AnimatorIsMovingParameter, false);
                return;
            }

            animator.SetFloat(AnimatorMovementParameter, inputMagnitude, 0.1f, Time.deltaTime);
            animator.SetBool(AnimatorIsMovingParameter, true);
            var movement = new Vector3(move.x, 0, move.y);

            /* Make Relative to Camera */
            var targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
        
            // Look the right way
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    
        private void OnAnimatorMove()
        {
            var velocity = animator.deltaPosition * speed;
            velocity.y = Physics.gravity.y * Time.deltaTime;
            controller.Move(velocity);
        }
    
        public void OnMove(InputAction.CallbackContext context)
        {
            move = context.ReadValue<Vector2>();
        }

        public void OnMoveCamera(InputAction.CallbackContext context)
        {
            // Could add some cool stuff here like having the character look in the direction of the camera.
            return;
        }

        private void OnEnable()
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }
    }
}
