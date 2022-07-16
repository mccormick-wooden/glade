using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Interfaces;
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
    /// Automatically skip dialogue
    /// </summary>
    [SerializeField]
    private bool skipDialogueStates = false;

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
    private IDamageable playerDamageModel;
    private CameraBlendEventDispatcher cameraBlendEventDispatcher;

    /// <summary>
    /// The triggerPlaneGameObjectName must be the name of an object with a TriggerPlane behavior
    /// </summary>
    [SerializeField]
    private string triggerPlaneGameObjectName = "TriggerPlane";
    private TriggerPlane outOfBoundsTriggerPlane;

    [SerializeField]
    private string dialogueCanvasName = "TreeSpiritDialogueCanvas";
    private Canvas dialogueCanvas;
    private DialogueController dialogueController;

    // For skipping
    private LongClickButton sceneSkipperButton;

    // DialogueStateStuff
    private List<TrainingState> dialogueStates = new List<TrainingState> { TrainingState.IntroDialogue, TrainingState.PostEnemyCombatDialogue, TrainingState.PostBeaconCombatDialogue };
    private Action<ICinemachineCamera> onCameraBlendToTrainingHostComplete = null;
    private Action onAllDialogueCompleted = null;
    private Action<ICinemachineCamera> onCameraBlendToPlayerComplete = null;

    // CombatStateStuff
    private Action<IDamageable, string, int> onCombatCompleted = null;
    private List<IDamageable> NextStateKillList = null;

    private GameObject beacon;
    private GameObject crystal;
    private GameObject enemy1;
    private GameObject enemy2;
    private GameObject enemy3;

    private Dictionary<TrainingState, List<string>> dialogueDictionary = new Dictionary<TrainingState, List<string>>
    {
        { TrainingState.IntroDialogue, new List<string>() { "Ah, Warden of The Glades! You've arrived just in time!",
                                                            "These gosh darn aliens are just causing the BIGGEST ruckus!",
                                                            "I really need you to do me a solid and clear them out of here. Couldya do that for me?",
                                                            "It looks like you brought your SWORD... Good! Do you remember how to use it? I guess it has been awhile.",
                                                            "When you see one of those pesky ALIENS, make sure you use RB/Left Click to swing that sucker until the alien is good and dead!",
                                                            "Same with the BEACONS! That's how the aliens are getting here I reckon.",
                                                            "Oh crap, here comes a Beacon now! Kill any aliens before you go for the Beacon, or I think you might open yourself to serious danger!",
                                                            "But don't worry too much - I'll heal you if your health gets too low! That's what Ancient Tree Spirit friends are for!",
                                                            "Oh, one more thing... Please try not to fall off this very tall and unnecessarily dangerous mesa. Teleporting Wardens back to safety is SO tacky ya know?"}
        },
        { TrainingState.PostEnemyCombatDialogue, new List<string>() { "Wow, you schmacked those fools!",
                                                                      "I doubt those are the last aliens we'll see - kill the Beacon before more aliens come out!",
                                                                      "You can use your SWORD again - remember, it's RB/Left Click to swing! But I'm sure you know that by now, otherwise we're probably in trouble...."}
        },
        { TrainingState.PostBeaconCombatDialogue, new List<string>() { "AND STAY OUT!!! Good job, Warden!",
                                                                       "Dang, it looks like the invasion is really getting started down there. You ready to get going? Think 45 seconds of combat training was enough?",
                                                                       "Don't answer that. Anyway, I'll go ahead and teleport you down to the invasion site so you can start clapping more aliens.",
                                                                       "Help me Obi-Warden Kenobi! You're my only hope! Good luck!!!" } 
        } // TODO: There should be an explanation of powerups / a state here once that stuff gets merged in.
    };


    protected void FixedUpdate()
    {
#if UNITY_EDITOR
        if (skipTraining)
        {
            skipTraining = false;
            UpdateNextGameState();
        }
#endif

        if (playerDamageModel.CurrentHp < 50)
        {
            playerDamageModel.Heal(playerDamageModel.MaxHp - playerDamageModel.CurrentHp);
            // TODO: Anim tree on heal
        }
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

        playerDamageModel = playerModel.GetComponent<IDamageable>();
        Utility.LogErrorIfNull(playerDamageModel, nameof(playerDamageModel));

        outOfBoundsTriggerPlane = GameObject.Find(triggerPlaneGameObjectName)?.GetComponentInChildren<TriggerPlane>();
        Utility.LogErrorIfNull(outOfBoundsTriggerPlane, nameof(outOfBoundsTriggerPlane));

        dialogueCanvas = GameObject.Find(dialogueCanvasName)?.GetComponent<Canvas>();
        Utility.LogErrorIfNull(dialogueCanvas, nameof(dialogueCanvas));

        dialogueController = dialogueCanvas.GetComponentInChildren<DialogueController>();
        Utility.LogErrorIfNull(dialogueController, nameof(dialogueController));

        sceneSkipperButton = GameObject.Find("SceneSkipperButton").GetComponent<LongClickButton>();
        Utility.LogErrorIfNull(sceneSkipperButton, nameof(sceneSkipperButton));

        beacon = GameObject.Find("CrashedBeacon");
        beacon.GetComponentInChildren<IDamageable>().enabled = false;
        crystal = GameObject.Find("Crystal");
        crystal.GetComponentInChildren<IDamageable>().enabled = false;
        enemy1 = GameObject.Find("Enemy1");
        enemy1.GetComponentInChildren<IDamageable>().enabled = false;
        enemy2 = GameObject.Find("Enemy2");
        enemy2.GetComponentInChildren<IDamageable>().enabled = false;
        enemy3 = GameObject.Find("Enemy3");
        enemy3.GetComponentInChildren<IDamageable>().enabled = false;

        SetEnemiesActiveInScene(false);
        #endregion

        #region helper event subscriptions
        outOfBoundsTriggerPlane.PlaneTriggered += OnOutOfBoundsPlaneTriggered;
        cameraBlendEventDispatcher.CameraBlendStarted += OnBlendToCameraStarted_DisableControlState;
        cameraBlendEventDispatcher.CameraBlendCompleted += OnBlendToCameraCompleted_EnableControlState;
        sceneSkipperButton.LongClickComplete += UpdateNextGameState;
        #endregion

        #region training event subscriptions
        trainingStateChanged += OnTrainingStateChanged_DialogueState;
        trainingStateChanged += OnTrainingStateChanged_EnemyCombat;
        trainingStateChanged += OnTrainingStateChanged_BeaconCombat;
        trainingStateChanged += OnTrainingStateChanged_End;
        #endregion

        #region set initial states
        currentTrainingState = TrainingState.Invalid;
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
        sceneSkipperButton.LongClickComplete -= UpdateNextGameState;

        trainingStateChanged -= OnTrainingStateChanged_DialogueState;
        trainingStateChanged -= OnTrainingStateChanged_EnemyCombat;
        trainingStateChanged -= OnTrainingStateChanged_BeaconCombat;
        trainingStateChanged -= OnTrainingStateChanged_End;
    }

    protected override void UpdateNextGameState()
    {
        GameManager.instance.UpdateGameState(GameState.Level1); // TODO: I'd like the tree spirit to open a portal that the warden can walk through to end the scene
    }

    #region training state callbacks
    private void OnTrainingStateChanged_DialogueState(TrainingState trainingState)
    {
        if (!dialogueStates.Contains(trainingState)) 
            return;

        List<string> dialogueList = !skipDialogueStates ? dialogueDictionary[trainingState] : new List<string>();

        onCameraBlendToTrainingHostComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == trainingHostVirtualCameraName)
            {
                cameraBlendEventDispatcher.CameraBlendCompleted -= onCameraBlendToTrainingHostComplete;

                dialogueCanvas.enabled = true;
                dialogueController.BeginDialogue(dialogueList);
            }
        };

        onAllDialogueCompleted = () =>
        {
            dialogueController.AllDialogueCompleted -= onAllDialogueCompleted;

            trainingHostVirtualCamera.enabled = false;
            dialogueCanvas.enabled = false;
        };

        onCameraBlendToPlayerComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == playerCameraName)
            {
                cameraBlendEventDispatcher.CameraBlendCompleted -= onCameraBlendToPlayerComplete;

                NextTrainingState();
            }
        };

        cameraBlendEventDispatcher.CameraBlendCompleted += onCameraBlendToTrainingHostComplete;
        dialogueController.AllDialogueCompleted += onAllDialogueCompleted;
        cameraBlendEventDispatcher.CameraBlendCompleted += onCameraBlendToPlayerComplete;

        StartDialogueState();
    }

    private void OnTrainingStateChanged_EnemyCombat(TrainingState trainingState)
    {
        if (trainingState != TrainingState.EnemyCombat) 
            return;

        onCombatCompleted = (IDamageable damageable, string name, int instanceId) =>
        {
            damageable.Died -= onCombatCompleted;

            if (NextStateKillList.All(d => d.IsDead))
                NextTrainingState();
        };

        SetEnemiesActiveInScene(true);
        SetupNextStateKillList(enemy1.GetComponentInChildren<IDamageable>(), enemy2.GetComponentInChildren<IDamageable>(), enemy3.GetComponentInChildren<IDamageable>());
    }

    private void OnTrainingStateChanged_BeaconCombat(TrainingState trainingState)
    {
        if (trainingState != TrainingState.BeaconCombat) 
            return;

        onCombatCompleted = (IDamageable damageable, string name, int instanceId) =>
        {
            damageable.Died -= onCombatCompleted;

            if (NextStateKillList.All(d => d.IsDead))
                NextTrainingState();
        };

        SetupNextStateKillList(beacon.GetComponentInChildren<IDamageable>(), crystal.GetComponentInChildren<IDamageable>());
    }

    private void OnTrainingStateChanged_End(TrainingState trainingState)
    {
        if (trainingState != TrainingState.End) 
            return;

        UpdateNextGameState();
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
        TrainingState nextState;

        switch (currentTrainingState)
        {
            case TrainingState.Invalid:
                nextState = TrainingState.IntroDialogue;
                break;
            case TrainingState.IntroDialogue:
                nextState = TrainingState.EnemyCombat;
                break;
            case TrainingState.EnemyCombat:
                nextState = TrainingState.PostEnemyCombatDialogue;
                break;
            case TrainingState.PostEnemyCombatDialogue:
                nextState = TrainingState.BeaconCombat;
                break;
            case TrainingState.BeaconCombat:
                nextState = TrainingState.PostBeaconCombatDialogue;
                break;
            case TrainingState.PostBeaconCombatDialogue:
                nextState = TrainingState.End;
                break;
            default:
                throw new NotImplementedException("Training broke!"); 
        }

        trainingStateChanged.Invoke(currentTrainingState = nextState);
    }

    private void StartDialogueState()
    {
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

    private void SetEnemiesActiveInScene(bool value)
    {
        beacon.SetActive(value);
        crystal.SetActive(value);
        enemy1.SetActive(value);
        enemy2.SetActive(value);
        enemy3.SetActive(value);
    }

    private void SetupNextStateKillList(params IDamageable[] toKill)
    {
        NextStateKillList = new List<IDamageable>(toKill);
        NextStateKillList.ForEach(k =>
        {
            k.enabled = true;
            k.Died += onCombatCompleted;
        });
    }
    #endregion
}
