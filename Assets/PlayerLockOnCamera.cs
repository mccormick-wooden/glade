using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerLockOnCamera : MonoBehaviour
{
    private CinemachineFreeLook thirdPersonCamera;
    private CinemachineVirtualCamera lockOnCamera;

    private Transform lockOnTarget;

    private bool isLockingOn;

    private void Awake()
    {
        lockOnCamera = GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(lockOnCamera, "lockOnCamera");
        thirdPersonCamera = FindObjectOfType<CinemachineFreeLook>();
        Utility.LogErrorIfNull(thirdPersonCamera, "thirdPersonCamera");
    }

    public void EnableLockOnCamera(Transform newLockOnTarget)
    {
        Debug.Log("Enabling Lock On Camera");

        lockOnCamera.LookAt = newLockOnTarget;

        lockOnCamera.enabled = true;
        thirdPersonCamera.enabled = false;
    }

    public void DisableLockOnCamera()
    {
        Debug.Log("Disabling Lock On Camera");
        thirdPersonCamera.enabled = true;
        lockOnCamera.enabled = false;
    }
}
