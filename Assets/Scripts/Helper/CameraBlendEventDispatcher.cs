using UnityEngine;
using Cinemachine;
using System;

[RequireComponent(typeof(CinemachineBrain))]
public class CameraBlendEventDispatcher : MonoBehaviour
{
    public Action<ICinemachineCamera> CameraBlendStarted;
    public Action<ICinemachineCamera> CameraBlendCompleted;

    private CinemachineBrain cineMachineBrain;

    private bool wasBlendingLastFrame;

    void Awake()
    {
        cineMachineBrain = GetComponent<CinemachineBrain>();
    }
    void Start()
    {
        wasBlendingLastFrame = false;
    }

    void Update()
    {
        if (cineMachineBrain.IsBlending)
        {
            if (!wasBlendingLastFrame)
            {
                CameraBlendStarted?.Invoke(cineMachineBrain.ActiveVirtualCamera);
            }

            wasBlendingLastFrame = true;
        }
        else
        {
            if (wasBlendingLastFrame)
            {
                CameraBlendCompleted?.Invoke(cineMachineBrain.ActiveVirtualCamera);
                wasBlendingLastFrame = false;
            }
        }
    }
}
