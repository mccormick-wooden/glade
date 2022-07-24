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

    private float verticalInput;
    private float horizontalInput;

    // New camera position and rotation magnitude based on player input. Zero
    // out the Z axis (we don't care about zooming in on/out of the player).
    // Cinemachine's rigs will handle the FOV for us anyway.
    private Vector3 newCamPosition => new Vector3(horizontalInput, verticalInput, 0f).normalized;
    private float rotationMagnitude => newCamPosition.magnitude;

    private void Awake()
    {
        
        GetCamera();
        SetupControls();
    }
    
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
            if (!thirdPersonCam.enabled)
            {
                return;
            }

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
        if (controls == null)
        {
            controls = new CharacterCameraControls();
        }
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        if (controls == null)
        {
            controls = new CharacterCameraControls();
        }
        controls.Gameplay.Disable();
    }
}
