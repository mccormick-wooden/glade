using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnIndicatorAnimation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float hoverFrequency;
    public float hoverAmplitude;

    private void FixedUpdate()
    {
        // Hover
        var newPosition = transform.position;
        newPosition.y += Mathf.Sin(Time.fixedTime * Mathf.PI * hoverFrequency) * hoverAmplitude;

        transform.position = newPosition;

        // Spin
        transform.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.fixedDeltaTime);
    }
}
