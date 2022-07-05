﻿using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TrainingStateManager : BaseStateManager
{
    enum TrainingState
    {
        Invalid = 0,
        IntroDialogue = 10,
        EnemyCombat = 20,
        PostEnemyCombatDialogue = 30,
        BeaconCombat = 40,
        PostBeaconCombatDialogue = 50,
        End = 60
    }

    private TrainingState currentTrainingState;

    private Action<TrainingState> trainingStateChanged;

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

    // DialogueStateCallbacks
    private Action<ICinemachineCamera> onCameraBlendToTrainingHostComplete = null;
    private Action onDialogueCompleted = null;
    private Action<ICinemachineCamera> onCameraBlendToPlayerComplete = null;


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

        #region general event subscriptions
        outOfBoundsTriggerPlane.PlaneTriggered += OnOutOfBoundsPlaneTriggered;
        cameraBlendEventDispatcher.CameraBlendStarted += OnBlendToCameraStarted_DisableControlState;
        cameraBlendEventDispatcher.CameraBlendCompleted += OnBlendToCameraCompleted_EnableControlState;

        trainingStateChanged += OnTrainingStateChanged_IntroDialogue;
        trainingStateChanged += OnTrainingStateChanged_EnemyCombat;
        trainingStateChanged += OnTrainingstateChanged_PostEnemyCombatDialogue;
        trainingStateChanged += OnTrainingStateChanged_BeaconCombat;
        trainingStateChanged += OnTraningStateChanged_PostBeaconCombatDialogue;
        trainingStateChanged += OnTrainingStateChanged_End;
        #endregion

        #region set initial states
        UpdateControlStateGracefully(enableControlState: false); // Don't allow control on scene start
        dialogueCanvas.enabled = false;
        trainingHostVirtualCamera.enabled = false;
        Invoke("KickItOff", time: 2);
        #endregion
    }

    protected override void OnSceneUnloaded()
    {
        outOfBoundsTriggerPlane.PlaneTriggered -= OnOutOfBoundsPlaneTriggered;
        cameraBlendEventDispatcher.CameraBlendStarted -= OnBlendToCameraStarted_DisableControlState;
        cameraBlendEventDispatcher.CameraBlendCompleted -= OnBlendToCameraCompleted_EnableControlState;

        trainingStateChanged -= OnTrainingStateChanged_IntroDialogue;
        trainingStateChanged -= OnTrainingStateChanged_EnemyCombat;
        trainingStateChanged -= OnTrainingstateChanged_PostEnemyCombatDialogue;
        trainingStateChanged -= OnTrainingStateChanged_BeaconCombat;
        trainingStateChanged -= OnTraningStateChanged_PostBeaconCombatDialogue;
        trainingStateChanged -= OnTrainingStateChanged_End;

    }

    #region state specific callbacks
    private void OnTrainingStateChanged_IntroDialogue(TrainingState trainingState)
    {
        if (trainingState != TrainingState.IntroDialogue) 
            return;

        List<string> dialogueList = new List<string>()
        {
            "Ah, Warden of The Glades! You've arrived just in time.",
            "These gosh darn aliens are just causing the biggest ruckus.",
            "I really need you to do me a solid and clear them out of here.",
            "It looks like you brought your SWORD... Good! Do you remember how to use it? I guess it has been awhile.",
            "When you see one of those pesky ALIENS, make sure you use RB/Left Click to kill the heck out of it!",
            "Same with the BEACONS! That's how the aliens are getting here I reckon.",
            "Oh crap, here comes one now! Kill any aliens before you go for the Beacon, or I think you might open yourself to serious danger!"
        };

        onCameraBlendToTrainingHostComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == trainingHostVirtualCameraName)
            {
                dialogueCanvas.enabled = true;
                dialogueController.BeginDialogue(dialogueList);
            }

            cameraBlendEventDispatcher.CameraBlendCompleted -= onCameraBlendToTrainingHostComplete;
        };

        onDialogueCompleted = () =>
        {
            trainingHostVirtualCamera.enabled = false;
            dialogueCanvas.enabled = false;
            dialogueController.DialogueCompleted -= onDialogueCompleted;
        };

        onCameraBlendToPlayerComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == playerCameraName)
            {
                // spawn some shit
            }
            cameraBlendEventDispatcher.CameraBlendCompleted -= onCameraBlendToPlayerComplete;
        };

        StartDialogueState(onCameraBlendToTrainingHostComplete, onDialogueCompleted, onCameraBlendToPlayerComplete);
    }

    private void OnTrainingStateChanged_EnemyCombat(TrainingState trainingState)
    {
        if (trainingState != TrainingState.EnemyCombat) return;

        NextTrainingState();
    }

    private void OnTrainingstateChanged_PostEnemyCombatDialogue(TrainingState trainingState)
    {
        if (trainingState != TrainingState.PostEnemyCombatDialogue) return;

        //NextTrainingState();
    }

    private void OnTrainingStateChanged_BeaconCombat(TrainingState trainingState)
    {
        if (trainingState != TrainingState.BeaconCombat) return;

        //NextTrainingState();
    }

    private void OnTraningStateChanged_PostBeaconCombatDialogue(TrainingState trainingState)
    {
        if (trainingState != TrainingState.PostEnemyCombatDialogue) return;

        //NextTrainingState();
    }

    private void OnTrainingStateChanged_End(TrainingState trainingState)
    {
        if (trainingState != TrainingState.End) return;

        GameManager.instance.UpdateGameState(GameState.Level1);
    }
    #endregion

    #region general event callbacks
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
    #endregion

    #region helpers
    private void NextTrainingState()
    {
        switch (currentTrainingState)
        {
            case TrainingState.Invalid:
                trainingStateChanged.Invoke(currentTrainingState = TrainingState.IntroDialogue);
                break;
            case TrainingState.IntroDialogue:
                trainingStateChanged.Invoke(currentTrainingState = TrainingState.EnemyCombat);
                break;
            case TrainingState.EnemyCombat:
                trainingStateChanged.Invoke(currentTrainingState = TrainingState.PostEnemyCombatDialogue);
                break;
            case TrainingState.PostEnemyCombatDialogue:
                trainingStateChanged.Invoke(currentTrainingState = TrainingState.BeaconCombat);
                break;
            case TrainingState.BeaconCombat:
                trainingStateChanged.Invoke(currentTrainingState = TrainingState.PostBeaconCombatDialogue);
                break;
            case TrainingState.PostBeaconCombatDialogue:
                trainingStateChanged.Invoke(currentTrainingState = TrainingState.End);
                break;
            default:
                break;
        }
    }

    private void StartDialogueState(
    Action<ICinemachineCamera> onCameraBlendToTrainingHostComplete,
    Action onDialogueCompleted,
    Action<ICinemachineCamera> onCameraBlendToPlayerComplete)
    {
        cameraBlendEventDispatcher.CameraBlendCompleted += onCameraBlendToTrainingHostComplete;
        dialogueController.DialogueCompleted += onDialogueCompleted;
        cameraBlendEventDispatcher.CameraBlendCompleted += onCameraBlendToPlayerComplete;

        trainingHostVirtualCamera.enabled = true;
    }

    private void UpdateControlStateGracefully(bool enableControlState)
    {
        if (!enableControlState)
            playerScript.StopAnimMotion();
        playerScript.UpdateControlState(enableControlState);
    }

    private void KickItOff()
    {
        NextTrainingState();
    }
    #endregion
}
