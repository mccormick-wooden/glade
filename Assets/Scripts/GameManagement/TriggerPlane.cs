using System;
using UnityEngine;

public class TriggerPlane : MonoBehaviour
{
    public Action PlaneTriggered;

    private bool isTriggered;

    private void OnTriggerEnter()
    {
        if (isTriggered) 
            return;

        isTriggered = true;
        PlaneTriggered?.Invoke();
    }

    private void OnTriggerExit()
    {
        isTriggered = false;
    }
}
