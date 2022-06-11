using UnityEngine;

// Apparently, in Unity a billboard is an object that always faces the camera.
// Not in use now, but could be in use if we wanted to add health bars to enemies
public class BillboardController : MonoBehaviour
{
    // This is called last in the update process. With this we ensure that we
    // adjust the billboard to face the camera's most recent position for this
    // frame.
    void LateUpdate()
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 up = Camera.main.transform.up;
        transform.rotation = Quaternion.LookRotation(forward, up);
    }
}
