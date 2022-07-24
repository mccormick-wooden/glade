using Cinemachine;
using UnityEngine;

public class PlayerLockOnCamera : MonoBehaviour
{
    public GameObject lockOnIndicatorPreFab;
    private GameObject lockOnIndicatorInstance;

    [SerializeField] private float lockOnIndicatorYOffset = 0f;

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
        UpdateVisualizerPosition();
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

        SetLockOnIndicatorOffset(newLockOnTarget);

        EnableVisualizer();
    }

    private void SetLockOnIndicatorOffset(Transform lockOnTarget)
    {
        var targetCollider = lockOnTarget.GetComponent<Collider>();
        if (targetCollider != null)
        {
            // If we can get the collider place the indicator two times higher than the center of it
            var targetWorldPosition = lockOnTarget.position;
            var center = targetCollider.bounds.center; // World-space

            var diff = center.y - targetWorldPosition.y;

            lockOnIndicatorYOffset = 2 * diff;
        }
        else
        {
            // Approximation that sometimes works - we shouldn't encounter this situations because
            // we shouldn't be locking onto something that doesn't have a collider anyway
            lockOnIndicatorYOffset = lockOnTarget.localPosition.y * 2f;
        }
    }

    public void DisableLockOnCamera()
    {
        isLockingOn = false;
        thirdPersonCamera.enabled = true;
        lockOnCamera.enabled = false;

        lockOnIndicatorYOffset = 0f;

        DisableVisualizer();
    }

    public void UpdateLockOnCameraLookAt(Transform updatedLockOnTarget)
    {
        lockOnCamera.LookAt = updatedLockOnTarget;
        SetLockOnIndicatorOffset(updatedLockOnTarget);
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

    private void UpdateVisualizerPosition()
    {
        if (!isLockingOn)
        {
            return;
        }

        var target = lockOnCamera.LookAt.transform.position;

        var amplitude = lockOnIndicatorInstance.GetComponent<LockOnIndicatorAnimation>()?.hoverAmplitude ?? 0f;
        var newPosition = new Vector3(target.x, target.y + lockOnIndicatorYOffset + amplitude, target.z);

        lockOnIndicatorInstance.transform.position = newPosition;
    }
}
