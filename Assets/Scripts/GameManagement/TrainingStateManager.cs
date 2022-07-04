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
    /// The trainingHostVirtualCameraName must be the name of the virtual camera focused on the Training Host
    /// </summary>
    [SerializeField]
    private string trainingHostVirtualCameraName = "TrainingHostVirtualCamera";
    private CinemachineVirtualCamera trainingHostVirtualCamera;

    /// <summary>
    /// The playerCameraName must be the name of the main camera that follows the player
    /// </summary>
    [SerializeField]
    private string playerCameraName = "3rdPersonCamera";

    /// <summary>
    /// The playerGameObjectRootName must be the name of the physical player model.
    /// </summary>
    [SerializeField]
    private string playerModelGameObjectRootName = "PlayerModel";
    private GameObject playerModel;
    private Vector3 playerModelStartingPos;
    private Player playerScript;

    /// <summary>
    /// The triggerPlaneGameObjectName must be the name of an object with a TriggerPlane behavior
    /// </summary>
    [SerializeField]
    private string triggerPlaneGameObjectName = "TriggerPlane";
    private TriggerPlane outOfBoundsTriggerPlane;

    private void Update()
    {

    }

    protected void FixedUpdate()
    {
#if UNITY_EDITOR
        if (skipTraining)
        {
            skipTraining = false;
            GameManager.instance.UpdateGameState(GameState.Level1);
        }
#endif
    }

    protected override void OnSceneLoaded()
    {
        #region get dependencies
        trainingHostVirtualCamera = GameObject.Find(trainingHostVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(trainingHostVirtualCamera, nameof(trainingHostVirtualCamera));

        playerModel = GameObject.Find(playerModelGameObjectRootName);
        Utility.LogErrorIfNull(playerModel, nameof(playerModel));
        playerModelStartingPos = playerModel.transform.position;

        playerScript = playerModel.GetComponent<Player>();
        Utility.LogErrorIfNull(playerScript, nameof(playerScript));
        playerScript.UpdateControlStateGracefully(enableControlState: false); // Don't allow control on scene start

        outOfBoundsTriggerPlane = GameObject.Find(triggerPlaneGameObjectName)?.GetComponentInChildren<TriggerPlane>();
        Utility.LogErrorIfNull(outOfBoundsTriggerPlane, nameof(outOfBoundsTriggerPlane));
        #endregion

        #region event subscriptions
        outOfBoundsTriggerPlane.PlaneTriggered += OnOutOfBoundsPlaneTriggered;
        CameraBlendEventDispatcher.CameraBlendStarted += OnCameraBlendStarted;
        CameraBlendEventDispatcher.CameraBlendCompleted += OnCameraBlendCompleted;
        #endregion

        InvokeRepeating("testswitchcamera", 1f, 5f);
    }

    protected override void OnSceneUnloaded()
    {
        outOfBoundsTriggerPlane.PlaneTriggered -= OnOutOfBoundsPlaneTriggered;
        CameraBlendEventDispatcher.CameraBlendStarted -= OnCameraBlendStarted;
        CameraBlendEventDispatcher.CameraBlendCompleted -= OnCameraBlendCompleted;
    }

    private void OnOutOfBoundsPlaneTriggered()
    {
        GameManager.instance.InvokeTransition(midTransitionAction: () => playerModel.transform.position = playerModelStartingPos);
    }

    private void OnCameraBlendStarted(ICinemachineCamera activeCamera)
    {
        if (activeCamera.Name == trainingHostVirtualCameraName)
            playerScript.UpdateControlStateGracefully(enableControlState: false);
    }

    private void OnCameraBlendCompleted(ICinemachineCamera activeCamera)
    {
        if (activeCamera.Name == playerCameraName)
            playerScript.UpdateControlStateGracefully(enableControlState: true);
    }

    private void testswitchcamera()
    {
        trainingHostVirtualCamera.enabled = !trainingHostVirtualCamera.enabled;
    }
}
