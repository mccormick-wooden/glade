using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private string objectNameToTrack = "PlayerModel";
    private Transform trackedTransform;


    private void Start()
    {
        trackedTransform = GameObject.Find(objectNameToTrack)?.transform;
        Utility.LogErrorIfNull(trackedTransform, nameof(trackedTransform), $"Minimap can't find the thing to track: '{objectNameToTrack}'");
    }

    private void LateUpdate()
    {
        transform.position = new Vector3 { x = trackedTransform.position.x, y = trackedTransform.position.y + 100, z = trackedTransform.position.z };

        // Rotate with camera
        Vector3 targetRotation = transform.rotation.eulerAngles;
        targetRotation.y = Camera.main.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(targetRotation.x, targetRotation.y, targetRotation.z);
    }
}
