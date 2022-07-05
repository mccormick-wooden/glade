using System;
using UnityEngine;

public class TriggerPlane : MonoBehaviour
{
    public Action PlaneTriggered;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("PlaneTriggered");
        PlaneTriggered?.Invoke();
    }
}
