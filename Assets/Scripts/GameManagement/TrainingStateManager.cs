using Cinemachine;
using UnityEngine;

public class TrainingStateManager : BaseStateManager
{
    /// <summary>
    /// Controls whether the training will be skipped
    /// </summary>
    [Header("Scene Settings")]
    [SerializeField]
    private bool skipTraining = false;

    /// <summary>
    /// todo
    /// </summary>
    [SerializeField]
    private string trainingHostVirtualCameraName = "TrainingHostVirtualCamera";

    private CinemachineVirtualCamera trainingHostVirtualCamera;

    //[SerializeField]
    //private string mainExitRootName = "MainExitGame";

    protected void FixedUpdate()
    {
#if UNITY_EDITOR
        if (skipTraining)
            GameManager.UpdateGameState(GameState.Level1);
#endif
    }

    protected override void OnSceneLoaded()
    {
        trainingHostVirtualCamera = GameObject.Find(trainingHostVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(trainingHostVirtualCamera, nameof(trainingHostVirtualCamera));

        InvokeRepeating("testswitchcamera", 1f, 2f);
    }

    protected override void OnSceneUnloaded()
    {
    }

    private void testswitchcamera()
    {
        trainingHostVirtualCamera.enabled = !trainingHostVirtualCamera.enabled;
    }
}
