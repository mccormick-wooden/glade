using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerManager : MonoBehaviour
{
    public GameObject PositionSync;

    private void Awake()
    {
        if (PositionSync == null)
            PositionSync = GameObject.FindGameObjectWithTag("Player");

        Utility.LogErrorIfNull(PositionSync, nameof(PositionSync), "Requires a gameobject to sync position with each frame");
    }

    void LateUpdate()
    {
        if (PositionSync != null)
            transform.position = PositionSync.transform.position;

        transform.rotation = Camera.main.transform.rotation;
    }
}
