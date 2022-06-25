using System;
using UnityEngine;

/* 
* Please note that the `FireProjectileScript.cs` depends on the CrashedBeacon class to find crashed beacon game objects**
*/
namespace Beacons
{
    public class CrashedBeacon : MonoBehaviour
    {
        private Vector3 position;

        [SerializeField] private float rotationSpeed;
        [SerializeField] private float hoverFrequency;
        [SerializeField] private float hoverAmplitude;

        private void Start()
        {
            position = transform.position;
            position.y += hoverAmplitude;
        }

        private void Update()
        {
            Hover();
        }

        private void Hover()
        {
            transform.Rotate(new Vector3(0f, Time.deltaTime * rotationSpeed, 0f));

            var newPosition = position;
            newPosition.y += Mathf.Sin (Time.fixedTime * Mathf.PI * hoverFrequency) * hoverAmplitude;
 
            transform.position = newPosition;
        }
    }
}
