using System;
using UnityEngine;

public class TriggerPlane : MonoBehaviour
{
    public Action<Collider> PlaneTriggered;

    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) 
            return;

        isTriggered = true;
        PlaneTriggered?.Invoke(other);
    }

    private void OnTriggerExit()
    {
        isTriggered = false;
    }
}
