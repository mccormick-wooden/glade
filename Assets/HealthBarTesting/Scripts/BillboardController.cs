using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dz
{
    // Apparently, in Unity a billboard is an object that always faces the camera.
    public class BillboardController : MonoBehaviour
    {
        public Transform MyCamera;

        // This is called last in the update process. With this we ensure that we
        // adjust the billboard to face the camera's most recent position for this
        // frame.
        void LateUpdate()
        {
            transform.LookAt(transform.position + MyCamera.forward);
        }
    }
}

