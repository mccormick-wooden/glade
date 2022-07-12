using System;
using UnityEngine;
using Cinemachine;
using PlayerBehaviors;

public class ThirdPersonCamera : MonoBehaviour
{
    // Camera Controls.
    private CharacterCameraControls controls;
    private GameObject cinemachine;
    private CinemachineFreeLook thirdPersonCam;

    private Player player;
    private PlayerCombat playerCombat;

    private float verticalInput;
    private float horizontalInput;

    // New camera position and rotation magnitude based on player input. Zero
    // out the Z axis (we don't care about zooming in on/out of the player).
    // Cinemachine's rigs will handle the FOV for us anyway.
    private Vector3 newCamPosition => new Vector3(horizontalInput, verticalInput, 0f).normalized;
    private float rotationMagnitude => newCamPosition.magnitude;

    private bool isLockingOn;
    
    private void Awake()
    {
        
        GetCamera();
        SetupControls();
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            playerCombat = player.GetComponent<PlayerCombat>();
        }
    }

    # region Lock-On Logic

    public void EnableLockOn()
    {
        Debug.Log("Camera is enabling lock on");
        isLockingOn = true;
        if (thirdPersonCam != null)
        {
            thirdPersonCam.LookAt = playerCombat.currentLockedOnTarget;
        }
    }

    public void DisableLockOn()
    {
        Debug.Log("Camera is disabling lock on");
        isLockingOn = false;
    }

    #endregion

    private void GetCamera()
    {
        cinemachine = GameObject.Find("CameraParent/3rdPersonCamera");
        Utility.LogErrorIfNull(cinemachine, "3rd Person Camera");
        thirdPersonCam = cinemachine.GetComponent<CinemachineFreeLook>();
    }

    void SetupControls()
    {
        controls = new CharacterCameraControls();

        controls.Gameplay.Look.performed += ctx =>
        {
            Vector2 rightStick = ctx.ReadValue<Vector2>();
            horizontalInput = rightStick.x;
            verticalInput = rightStick.y;

            // Ignore any negligible rotation.
            if (rotationMagnitude < 0.1)
            {
                horizontalInput = 0f;
                verticalInput = 0f;
            }

            thirdPersonCam.m_XAxis.m_InputAxisValue = horizontalInput;
            thirdPersonCam.m_YAxis.m_InputAxisValue = verticalInput;
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
}
