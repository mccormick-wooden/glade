using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerLockOnCamera : MonoBehaviour
{
    public GameObject lockOnIndicatorPreFab;
    private GameObject lockOnIndicatorInstance;
    private float lockOnIndicatorYOffset = 0f;

    private CinemachineFreeLook thirdPersonCamera;
    private CinemachineVirtualCamera lockOnCamera;

    private bool isLockingOn;

    private void Awake()
    {
        lockOnCamera = GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(lockOnCamera, "lockOnCamera");
        thirdPersonCamera = FindObjectOfType<CinemachineFreeLook>();
        Utility.LogErrorIfNull(thirdPersonCamera, "thirdPersonCamera");
    }

    private void FixedUpdate()
    {
        UpdateVisualizer();
    }

    public void EnableLockOnCamera(Transform newLockOnTarget)
    {
        if (newLockOnTarget == null)
        {
            return;
        }

        isLockingOn = true;
        lockOnCamera.LookAt = newLockOnTarget;
        lockOnCamera.enabled = true;
        thirdPersonCamera.enabled = false;

        lockOnIndicatorYOffset = newLockOnTarget.localPosition.y * 2f;

        EnableVisualizer();
    }

    public void DisableLockOnCamera()
    {
        isLockingOn = false;
        thirdPersonCamera.enabled = true;
        lockOnCamera.enabled = false;

        lockOnIndicatorYOffset = 0f;

        DisableVisualizer();
    }

    private void EnableVisualizer()
    {
        if (lockOnIndicatorInstance == null)
        {
            lockOnIndicatorInstance =
                Instantiate(lockOnIndicatorPreFab, lockOnCamera.LookAt.position, Quaternion.identity);
        }
        else
        {
            lockOnIndicatorInstance.SetActive(true);
        }
    }

    private void DisableVisualizer()
    {
        lockOnIndicatorInstance.SetActive(false);
    }

    private void UpdateVisualizer()
    {
        if (!isLockingOn)
        {
            return;
        }

        var target = lockOnCamera.LookAt.transform.position;
        var newPosition = new Vector3(target.x, target.y + lockOnIndicatorYOffset, target.z);

        lockOnIndicatorInstance.transform.position = newPosition;
    }
}
