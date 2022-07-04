using System.Collections.Generic;
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
    private CameraBlendEventDispatcher cameraBlendEventDispatcher;

    /// <summary>
    /// The triggerPlaneGameObjectName must be the name of an object with a TriggerPlane behavior
    /// </summary>
    [SerializeField]
    private string triggerPlaneGameObjectName = "TriggerPlane";
    private TriggerPlane outOfBoundsTriggerPlane;

    [SerializeField]
    private string dialogueControllerName = "DialogueController";
    private DialogueController dialogueController;

    [SerializeField]
    private string dialogueCanvasName = "TreeSpiritDialogueCanvas";
    private Canvas dialogueCanvas;

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
        #region get references
        trainingHostVirtualCamera = GameObject.Find(trainingHostVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(trainingHostVirtualCamera, nameof(trainingHostVirtualCamera));

        playerModel = GameObject.Find(playerModelGameObjectRootName);
        Utility.LogErrorIfNull(playerModel, nameof(playerModel));
        playerModelStartingPos = playerModel.transform.position;

        cameraBlendEventDispatcher = playerModel.transform.parent.GetComponentInChildren<CameraBlendEventDispatcher>();
        Utility.LogErrorIfNull(cameraBlendEventDispatcher, nameof(cameraBlendEventDispatcher), "Need a camera blend event dispatcher somewhere");

        playerScript = playerModel.GetComponent<Player>();
        Utility.LogErrorIfNull(playerScript, nameof(playerScript));

        outOfBoundsTriggerPlane = GameObject.Find(triggerPlaneGameObjectName)?.GetComponentInChildren<TriggerPlane>();
        Utility.LogErrorIfNull(outOfBoundsTriggerPlane, nameof(outOfBoundsTriggerPlane));

        dialogueController = GameObject.Find(dialogueControllerName)?.GetComponent<DialogueController>();
        Utility.LogErrorIfNull(dialogueController, nameof(dialogueController));

        dialogueCanvas = GameObject.Find(dialogueCanvasName)?.GetComponent<Canvas>();
        Utility.LogErrorIfNull(dialogueCanvas, nameof(dialogueCanvas));
        #endregion

        #region event subscriptions
        outOfBoundsTriggerPlane.PlaneTriggered += OnOutOfBoundsPlaneTriggered;
        cameraBlendEventDispatcher.CameraBlendStarted += OnBlendToCameraStarted_DisableControlState;
        cameraBlendEventDispatcher.CameraBlendCompleted += OnBlendToCameraCompleted_EnableControlState;

        cameraBlendEventDispatcher.CameraBlendCompleted += BeginDialogue;
        #endregion

        #region set initial states
        UpdateControlStateGracefully(enableControlState: false); // Don't allow control on scene start
        dialogueCanvas.enabled = false;
        trainingHostVirtualCamera.enabled = false;
        #endregion

        Invoke("testswitchcamera", 1f);

    }

    protected override void OnSceneUnloaded()
    {
        outOfBoundsTriggerPlane.PlaneTriggered -= OnOutOfBoundsPlaneTriggered;
        cameraBlendEventDispatcher.CameraBlendStarted -= OnBlendToCameraStarted_DisableControlState;
        cameraBlendEventDispatcher.CameraBlendCompleted -= OnBlendToCameraCompleted_EnableControlState;

        cameraBlendEventDispatcher.CameraBlendCompleted -= BeginDialogue;


        CancelInvoke(); // Get Rid of this later
    }

    private void OnOutOfBoundsPlaneTriggered()
    {
        GameManager.instance.InvokeTransition(midTransitionAction: () => playerModel.transform.position = playerModelStartingPos);
    }

    private void OnBlendToCameraStarted_DisableControlState(ICinemachineCamera activeCamera)
    {
        if (activeCamera.Name == trainingHostVirtualCameraName)
        {
            UpdateControlStateGracefully(enableControlState: false);
        }
    }

    private void OnBlendToCameraCompleted_EnableControlState(ICinemachineCamera activeCamera)
    {
        if (activeCamera.Name == playerCameraName) 
        {
            UpdateControlStateGracefully(enableControlState: true);
        }
    }

    private void BeginDialogue(ICinemachineCamera activeCamera)
    {
        if (activeCamera.Name == trainingHostVirtualCameraName)
        {
            dialogueCanvas.enabled = true;
            dialogueController.BeginDialogue(new List<string> 
            { 
                "Hello mr glade man.", 
                "I am actually a real live tree",
                "please water me every day, so that i may grow tall" 
            });
        }
    }

    #region helpers
    private void UpdateControlStateGracefully(bool enableControlState)
    {
        if (!enableControlState)
            playerScript.StopAnimMotion();
        playerScript.UpdateControlState(enableControlState);
    }

    private void testswitchcamera()
    {
        trainingHostVirtualCamera.enabled = true;
    }
    #endregion
}
