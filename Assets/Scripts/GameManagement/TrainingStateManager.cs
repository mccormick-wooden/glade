﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Abstract;
using Assets.Scripts.Interfaces;
using Beacons;
using Cinemachine;
using PowerUps;
using UnityEngine;

public class TrainingStateManager : BaseStateManager
{
    enum TrainingState
    {
        Invalid = 0,
        GeneralIntroDialogue = 10,
        EnemyIntroDialogue = 11,
        CrystalIntroDialogue = 12,
        BeaconIntroDialogue = 13,
        EnemyCombat = 20,
        PostEnemyCombatDialogue = 30,
        CrystalCombat = 31,
        PostCrystalCombatDialogue = 32,
        BeaconCombat = 40,
        PostBeaconCombatDialogue = 50,
        PowerUp = 51,
        PostPowerUpDialogue = 52,
        End = 60
    }

    private TrainingState currentTrainingState;

    private Action<TrainingState> trainingStateChanged;

    /// <summary>
    /// The trainingHostVirtualCameraName must be the name of the virtual camera focused on the Training Host
    /// </summary>
    [SerializeField]
    private string trainingHostVirtualCameraName = "TrainingHostVirtualCamera";
    private CinemachineVirtualCamera trainingHostVirtualCamera;

    /// <summary>
    /// The enemyVirtualCameraName must be the name of the virtual camera focused on the Training enemies
    /// </summary>
    [SerializeField]
    private string enemyVirtualCameraName = "EnemyVirtualCamera";
    private CinemachineVirtualCamera enemyVirtualCamera;

    /// <summary>
    /// The crystalVirtualCameraName must be the name of the virtual camera focused on the Crystal enemies
    /// </summary>
    [SerializeField]
    private string crystalVirtualCameraName = "CrystalVirtualCamera";
    private CinemachineVirtualCamera crystalVirtualCamera;

    /// <summary>
    /// The beaconVirtualCameraName must be the name of the virtual camera focused on the Beacon enemies
    /// </summary>
    [SerializeField]
    private string beaconVirtualCameraName = "BeaconVirtualCamera";
    private CinemachineVirtualCamera beaconVirtualCamera;

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
    private CrystalDamageEffect playerCrystalDamageEffect;
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

    /// <summary>
    /// Beacon land audio
    /// </summary>
    [SerializeField]
    protected AudioClip beaconLandAudio;

    // For skipping
    private LongClickButton sceneSkipperButton;

    // DialogueStateStuff
    private List<TrainingState> genericDialogueStates = new List<TrainingState> { TrainingState.PostEnemyCombatDialogue, TrainingState.PostCrystalCombatDialogue, TrainingState.PostBeaconCombatDialogue };
    private Action<ICinemachineCamera> onCurrentStateCameraBlendComplete = null;
    private Action onAllDialogueCompleted = null;
    private Action<ICinemachineCamera> onNextStateCameraBlendComplete = null;

    // CombatStateStuff
    private float postCombatWait = 1f; // let combat animations finish before blending camera
    private Action<IDamageable, string, int> onCombatCompleted = null;
    private List<IDamageable> NextStateKillList = null;

    private GameObject beacon;
    private GameObject crystal;
    private GameObject enemy1;
    private GameObject enemy2;
    private GameObject enemy3;
    private List<GameObject> AllEnemies;
    private List<GameObject> DynamicEnemies;
    private List<GameObject> StaticEnemies;

    // Pickup
    private Action<BasePowerUp> onPowerUpApplied = null;
    private BasePowerUp appliedPowerUp = null;
    private PowerUpMenu powerUpMenu;

    private Dictionary<TrainingState, List<string>> dialogueDictionary = new Dictionary<TrainingState, List<string>>
    {
        //{ TrainingState.IntroDialogue, new List<string>() { "Ah, Warden of The Glades! You've arrived just in time!",
        //                                                    "These gosh darn aliens are just causing the BIGGEST ruckus!",
        //                                                    "I really need you to do me a solid and clear them out of here. Couldya do that for me?",
        //                                                    "It looks like you brought your SWORD... Good! Do you remember how to use it? I guess it has been awhile.",
        //                                                    "When you see on of those pesky ALIENS, just run right up to it! Use the left stick or WASD to move. Don't be shy!",
        //                                                    "Then once you're close, make sure you use RB/Left Click to swing that sucker until the alien is good and dead!",
        //                                                    "Same with the BEACONS! That's how the aliens are getting here I reckon.",
        //                                                    "Oh crap, here comes a Beacon now! Kill any aliens before you go for the Beacon, or I think you might open yourself to serious danger!",
        //                                                    "But don't worry too much - I'll heal you if your health gets too low! That's what Ancient Tree Spirit friends are for!",
        //                                                    "Oh, one more thing... Please try not to fall off this very tall and unnecessarily dangerous mesa. Teleporting Wardens back to safety is SO tacky ya know?"}
        //},
        { TrainingState.GeneralIntroDialogue, new List<string>() 
            { 
                "Ah, Warden of The Glades! You've arrived just in time!",
                "These gosh darn aliens are just causing the BIGGEST ruckus!",
                "I really need you to do me a solid and clear them out of here. Couldya do that for me?",
                "It looks like you brought your SWORD... Good! Do you remember how to use it? I guess it has been awhile.",
                "When you see on of those pesky ALIENS, just run right up to it! Use the LEFT STICK or WASD to move. Don't be shy!",
                "Then once you're close, make sure you use RB or LEFT CLICK to swing that sucker until the alien is good and dead!",
                "Same with the BEACONS! That's how the aliens are getting here I reckon.",
                "Oh crap, here comes a Beacon now!"
            }
        },
        { TrainingState.EnemyIntroDialogue, new List<string>()
            {
                $"{TrainingState.EnemyIntroDialogue} placeholder"
            } 
        },
        { TrainingState.CrystalIntroDialogue, new List<string>()
            {
                $"{TrainingState.CrystalIntroDialogue} placeholder"
            }
        },
        { TrainingState.BeaconIntroDialogue, new List<string>()
            {
                $"{TrainingState.BeaconIntroDialogue} placeholder"
            }
        },
        { TrainingState.PostEnemyCombatDialogue, new List<string>() 
            { 
                "Wow, you schmacked those fools!",
                "I doubt those are the last aliens we'll see - kill the Beacon before more aliens come out!",
                "You can use your SWORD again - remember, it's RB/Left Click to swing! But I'm sure you know that by now, otherwise we're probably in trouble...."
            }
        },
        { TrainingState.PostCrystalCombatDialogue, new List<string>()
            {
                $"{TrainingState.PostCrystalCombatDialogue} placeholder"
            }
        },
        { TrainingState.PostBeaconCombatDialogue, new List<string>() 
            { 
                "AND STAY OUT!!! Good job, Warden!",
                "Dang, it looks like the invasion is really getting started down there. You ready to get going? Think 45 seconds of combat training was enough?",
                "Don't answer that. Anyway, I'll go ahead and teleport you down to the invasion site so you can start clapping more aliens.",
                "Help me Obi-Warden Kenobi! You're my only hope! Good luck!!!" 
            } 
        },
        { TrainingState.PostPowerUpDialogue, new List<string>()
            {
                $"{TrainingState.PostPowerUpDialogue} placeholder"
            }
        }
    };

    private void DisableVirtualCameras()
    {
        trainingHostVirtualCamera.enabled = false;
        enemyVirtualCamera.enabled = false;
        crystalVirtualCamera.enabled = false;
        beaconVirtualCamera.enabled = false;
    }

    protected override void OnSceneLoaded()
    {
        #region get references
        trainingHostVirtualCamera = GameObject.Find(trainingHostVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(trainingHostVirtualCamera, nameof(trainingHostVirtualCamera));

        enemyVirtualCamera = GameObject.Find(enemyVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(enemyVirtualCamera, nameof(enemyVirtualCamera));

        crystalVirtualCamera = GameObject.Find(crystalVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(crystalVirtualCamera, nameof(crystalVirtualCamera));

        beaconVirtualCamera = GameObject.Find(beaconVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(beaconVirtualCamera, nameof(beaconVirtualCamera));

        playerModel = GameObject.Find(playerModelGameObjectRootName);
        Utility.LogErrorIfNull(playerModel, nameof(playerModel));
        playerModelStartingPos = playerModel.transform.position;
        playerCrystalDamageEffect = playerModel.GetComponentInChildren<CrystalDamageEffect>();

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

        var powerUpMenuParent = GameObject.Find("PowerUpMenu");
        powerUpMenu = powerUpMenuParent.GetComponentInChildren<PowerUpMenu>();
        Utility.LogErrorIfNull(powerUpMenu, nameof(powerUpMenu));
        powerUpMenuParent.SetActive(false);

        beacon = GameObject.Find("CrashedBeacon");
        crystal = GameObject.Find("Crystal");
        enemy1 = GameObject.Find("Enemy1");
        enemy2 = GameObject.Find("Enemy2");
        enemy3 = GameObject.Find("Enemy3");

        AllEnemies = new List<GameObject> { beacon, crystal, enemy1, enemy2, enemy3 };
        DynamicEnemies = new List<GameObject> { enemy1, enemy2, enemy3 };
        StaticEnemies = new List<GameObject> { beacon, crystal };

        SetBehaviorActiveInScene<BaseEnemy>(DynamicEnemies, value: false);
        SetBehaviorActiveInScene<BaseDamageable>(AllEnemies, value: false);
        SetActiveInScene(AllEnemies, value: false);
        #endregion

        #region helper event subscriptions
        outOfBoundsTriggerPlane.PlaneTriggered += OnOutOfBoundsPlaneTriggered;
        cameraBlendEventDispatcher.CameraBlendStarted += OnBlendToCameraStarted_DisableControlState;
        cameraBlendEventDispatcher.CameraBlendCompleted += OnBlendToCameraCompleted_EnableControlState;
        sceneSkipperButton.LongClickComplete += UpdateNextGameState;
        #endregion

        #region training event subscriptions
        trainingStateChanged += OnTrainingStateChanged_IntroDialogueState;
        trainingStateChanged += OnTrainingStateChanged_EnemyIntroDialogueState;
        trainingStateChanged += OnTrainingStateChanged_CrystalIntroDialogueState;
        trainingStateChanged += OnTrainingStateChanged_BeaconIntroDialogueState;
        trainingStateChanged += OnTrainingStateChanged_GenericDialogueState;
        trainingStateChanged += OnTrainingStateChanged_EnemyCombat;
        trainingStateChanged += OnTrainingStateChanged_CrystalCombat;
        trainingStateChanged += OnTrainingStateChanged_BeaconCombat;
        trainingStateChanged += OnTrainingStateChanged_PowerUp;
        trainingStateChanged += OnTrainingStateChanged_PostPowerUpDialogue;
        trainingStateChanged += OnTrainingStateChanged_End;
        #endregion

        #region set initial states
        currentTrainingState = TrainingState.Invalid;
        UpdateControlStateGracefully(enableControlState: false); // Don't allow control on scene start
        dialogueCanvas.enabled = false;
        DisableVirtualCameras();
        Invoke(methodName: "NextTrainingState", time: 2);
        #endregion
    }

    protected override void OnSceneUnloaded()
    {
        outOfBoundsTriggerPlane.PlaneTriggered -= OnOutOfBoundsPlaneTriggered;
        cameraBlendEventDispatcher.CameraBlendStarted -= OnBlendToCameraStarted_DisableControlState;
        cameraBlendEventDispatcher.CameraBlendCompleted -= OnBlendToCameraCompleted_EnableControlState;
        sceneSkipperButton.LongClickComplete -= UpdateNextGameState;

        trainingStateChanged -= OnTrainingStateChanged_IntroDialogueState;
        trainingStateChanged -= OnTrainingStateChanged_EnemyIntroDialogueState;
        trainingStateChanged -= OnTrainingStateChanged_CrystalIntroDialogueState;
        trainingStateChanged -= OnTrainingStateChanged_BeaconIntroDialogueState;
        trainingStateChanged -= OnTrainingStateChanged_GenericDialogueState;
        trainingStateChanged -= OnTrainingStateChanged_EnemyCombat;
        trainingStateChanged -= OnTrainingStateChanged_CrystalCombat;
        trainingStateChanged -= OnTrainingStateChanged_BeaconCombat;
        trainingStateChanged -= OnTrainingStateChanged_PowerUp;
        trainingStateChanged -= OnTrainingStateChanged_PostPowerUpDialogue;
        trainingStateChanged -= OnTrainingStateChanged_End;
    }

    protected override void UpdateNextGameState()
    {
        GameManager.instance.UpdateGameState(GameState.Level1); // TODO: I'd like the tree spirit to open a portal that the warden can walk through to end the scene
    }

    #region training state callbacks
    private void OnTrainingStateChanged_IntroDialogueState(TrainingState trainingState)
    {
        if (!trainingState.Equals(TrainingState.GeneralIntroDialogue))
            return;

        List<string> dialogueList = dialogueDictionary[trainingState] ?? new List<string> { "missing dialogue for state" };

        onCurrentStateCameraBlendComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == trainingHostVirtualCameraName)
            {
                cameraBlendEventDispatcher.CameraBlendCompleted -= onCurrentStateCameraBlendComplete;

                dialogueCanvas.enabled = true;
                dialogueController.BeginDialogue(dialogueList);
            }
        };

        onAllDialogueCompleted = () =>
        {
            dialogueController.AllDialogueCompleted -= onAllDialogueCompleted;

            DisableVirtualCameras();
            SetActiveInScene(AllEnemies, true);
            SetBehaviorActiveInScene<BaseEnemy>(DynamicEnemies, false);
            PlayCrashAudio();
            enemyVirtualCamera.enabled = true;
            dialogueCanvas.enabled = false;
        };

        onNextStateCameraBlendComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == enemyVirtualCameraName)
            {
                cameraBlendEventDispatcher.CameraBlendCompleted -= onNextStateCameraBlendComplete;

                NextTrainingState();
            }
        };

        cameraBlendEventDispatcher.CameraBlendCompleted += onCurrentStateCameraBlendComplete;
        dialogueController.AllDialogueCompleted += onAllDialogueCompleted;
        cameraBlendEventDispatcher.CameraBlendCompleted += onNextStateCameraBlendComplete;

        StartDialogueState(trainingHostVirtualCamera);
    }

    private void OnTrainingStateChanged_EnemyIntroDialogueState(TrainingState trainingState)
    {
        if (!trainingState.Equals(TrainingState.EnemyIntroDialogue))
            return;

        List<string> dialogueList = dialogueDictionary[trainingState] ?? new List<string> { "missing dialogue for state" };

        dialogueCanvas.enabled = true;
        dialogueController.BeginDialogue(dialogueList);

        onAllDialogueCompleted = () =>
        {
            dialogueController.AllDialogueCompleted -= onAllDialogueCompleted;

            DisableVirtualCameras();
            crystalVirtualCamera.enabled = true;
            dialogueCanvas.enabled = false;
        };

        onNextStateCameraBlendComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == crystalVirtualCameraName)
            {
                cameraBlendEventDispatcher.CameraBlendCompleted -= onNextStateCameraBlendComplete;
                NextTrainingState();
            }
        };

        dialogueController.AllDialogueCompleted += onAllDialogueCompleted;
        cameraBlendEventDispatcher.CameraBlendCompleted += onNextStateCameraBlendComplete;
    }

    private void OnTrainingStateChanged_CrystalIntroDialogueState(TrainingState trainingState)
    {
        if (!trainingState.Equals(TrainingState.CrystalIntroDialogue))
            return;

        List<string> dialogueList = dialogueDictionary[trainingState] ?? new List<string> { "missing dialogue for state" };

        dialogueCanvas.enabled = true;
        dialogueController.BeginDialogue(dialogueList);

        onAllDialogueCompleted = () =>
        {
            dialogueController.AllDialogueCompleted -= onAllDialogueCompleted;

            DisableVirtualCameras();
            beaconVirtualCamera.enabled = true;
            dialogueCanvas.enabled = false;
        };

        onNextStateCameraBlendComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == beaconVirtualCameraName)
            {
                cameraBlendEventDispatcher.CameraBlendCompleted -= onNextStateCameraBlendComplete;
                NextTrainingState();
            }
        };

        dialogueController.AllDialogueCompleted += onAllDialogueCompleted;
        cameraBlendEventDispatcher.CameraBlendCompleted += onNextStateCameraBlendComplete;
    }

    private void OnTrainingStateChanged_BeaconIntroDialogueState(TrainingState trainingState)
    {
        if (!trainingState.Equals(TrainingState.BeaconIntroDialogue))
            return;

        List<string> dialogueList = dialogueDictionary[trainingState] ?? new List<string> { "missing dialogue for state" };

        dialogueCanvas.enabled = true;
        dialogueController.BeginDialogue(dialogueList);

        onAllDialogueCompleted = () =>
        {
            dialogueController.AllDialogueCompleted -= onAllDialogueCompleted;

            DisableVirtualCameras();
            dialogueCanvas.enabled = false;
        };

        onNextStateCameraBlendComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == playerCameraName)
            {
                cameraBlendEventDispatcher.CameraBlendCompleted -= onNextStateCameraBlendComplete;
                SetBehaviorActiveInScene<BaseEnemy>(DynamicEnemies, true);
                NextTrainingState();
            }
        };

        dialogueController.AllDialogueCompleted += onAllDialogueCompleted;
        cameraBlendEventDispatcher.CameraBlendCompleted += onNextStateCameraBlendComplete;
    }

    private void OnTrainingStateChanged_GenericDialogueState(TrainingState trainingState)
    {
        if (!genericDialogueStates.Contains(trainingState)) 
            return;

        List<string> dialogueList = dialogueDictionary[trainingState] ?? new List<string> { "missing dialogue for state" };

        onCurrentStateCameraBlendComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == trainingHostVirtualCameraName)
            {
                cameraBlendEventDispatcher.CameraBlendCompleted -= onCurrentStateCameraBlendComplete;

                dialogueCanvas.enabled = true;
                dialogueController.BeginDialogue(dialogueList);
            }
        };

        onAllDialogueCompleted = () =>
        {
            dialogueController.AllDialogueCompleted -= onAllDialogueCompleted;

            DisableVirtualCameras();
            dialogueCanvas.enabled = false;
        };

        onNextStateCameraBlendComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == playerCameraName)
            {
                cameraBlendEventDispatcher.CameraBlendCompleted -= onNextStateCameraBlendComplete;

                NextTrainingState();
            }
        };

        cameraBlendEventDispatcher.CameraBlendCompleted += onCurrentStateCameraBlendComplete;
        dialogueController.AllDialogueCompleted += onAllDialogueCompleted;
        cameraBlendEventDispatcher.CameraBlendCompleted += onNextStateCameraBlendComplete;

        StartDialogueState(trainingHostVirtualCamera);
    }

    private void OnTrainingStateChanged_EnemyCombat(TrainingState trainingState)
    {
        if (trainingState != TrainingState.EnemyCombat) 
            return;

        onCombatCompleted = (IDamageable damageable, string name, int instanceId) =>
        {
            damageable.Died -= onCombatCompleted;

            if (NextStateKillList.All(d => d.IsDead))
                Invoke("NextTrainingState", postCombatWait);           
        };

        SetupNextStateKillList(DynamicEnemies);
    }

    private void OnTrainingStateChanged_CrystalCombat(TrainingState trainingState)
    {
        if (trainingState != TrainingState.CrystalCombat)
            return;

        onCombatCompleted = (IDamageable damageable, string name, int instanceId) =>
        {
            damageable.Died -= onCombatCompleted;

            if (NextStateKillList.All(d => d.IsDead))
            {
                Destroy(crystal); // it can still zap even after its dead, idk why
                Invoke("NextTrainingState", postCombatWait);
            }
        };

        SetupNextStateKillList(StaticEnemies.Where(o => o == crystal).ToList());
    }

    private string pickupName = "PowerUpPickup(Clone)";

    private void OnTrainingStateChanged_BeaconCombat(TrainingState trainingState)
    {
        if (trainingState != TrainingState.BeaconCombat) 
            return;

        onCombatCompleted = (IDamageable damageable, string name, int instanceId) =>
        {
            damageable.Died -= onCombatCompleted;

            if (NextStateKillList.All(d => d.IsDead))
            {
                GameObject.Find(pickupName).GetComponent<Collider>().isTrigger = false;
                Invoke("NextTrainingState", postCombatWait);
            }
        };

        SetupNextStateKillList(StaticEnemies.Where(o => o == beacon).ToList());
    }
    private void OnTrainingStateChanged_PowerUp(TrainingState trainingState)
    {
        if (trainingState != TrainingState.PowerUp)
            return;

        GameObject.Find(pickupName).GetComponent<Collider>().isTrigger = true;

        onPowerUpApplied = (BasePowerUp powerUp) =>
        {
            powerUpMenu.PowerUpApplied -= onPowerUpApplied;
            appliedPowerUp = powerUp;
            NextTrainingState();
        };

        powerUpMenu.PowerUpApplied += onPowerUpApplied;
    }

    private void OnTrainingStateChanged_PostPowerUpDialogue(TrainingState trainingState)
    {
        if (trainingState != TrainingState.PostPowerUpDialogue)
            return;

        List<string> dialogueList = dialogueDictionary[trainingState] ?? new List<string> { "missing dialogue for state" };

        string specialDialogue = string.Empty;
        switch (appliedPowerUp)
        {
            case DamageIncreasePowerUp t1:
                specialDialogue = "Wow, Warden you look JACKED now! That damage increase should definitely help.";
                break;
            case DamageResistPowerUp t2:
                specialDialogue = "Ooooo damage resistance! Get all damage spongey and whatnot, I like it!";
                break;
            case MaxHealthPowerUp t3:
                specialDialogue = "Increasing health is a good choice! Your skin looks healthier already!";
                break;
            default:
                specialDialogue = "Hmmm... That's a weird choice! Whatever, your funeral!";
                break;
        }

        dialogueList.Insert(0, specialDialogue);

        onCurrentStateCameraBlendComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == trainingHostVirtualCameraName)
            {
                cameraBlendEventDispatcher.CameraBlendCompleted -= onCurrentStateCameraBlendComplete;

                dialogueCanvas.enabled = true;
                dialogueController.BeginDialogue(dialogueList);
            }
        };

        onAllDialogueCompleted = () =>
        {
            dialogueController.AllDialogueCompleted -= onAllDialogueCompleted;

            DisableVirtualCameras();
            dialogueCanvas.enabled = false;
        };

        onNextStateCameraBlendComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == playerCameraName)
            {
                cameraBlendEventDispatcher.CameraBlendCompleted -= onNextStateCameraBlendComplete;

                NextTrainingState();
            }
        };

        cameraBlendEventDispatcher.CameraBlendCompleted += onCurrentStateCameraBlendComplete;
        dialogueController.AllDialogueCompleted += onAllDialogueCompleted;
        cameraBlendEventDispatcher.CameraBlendCompleted += onNextStateCameraBlendComplete;

        StartDialogueState(trainingHostVirtualCamera);
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
        trainingStateChanged.Invoke(currentTrainingState = currentTrainingState.Next());
    }

    private void StartDialogueState(CinemachineVirtualCamera dialogueCamera)
    {
        dialogueCamera.enabled = true;
    }

    private void UpdateControlStateGracefully(bool enableControlState)
    {
        if (!enableControlState)
            playerScript.StopAnimMotion();
        playerScript.UpdateControlState(enableControlState);
    }

    private void SetActiveInScene(List<GameObject> objs, bool value)
    {
        objs.ForEach(e => e.SetActive(value));
    }

    private void SetBehaviorActiveInScene<T>(List<GameObject> objs, bool value) where T : MonoBehaviour
    {
        objs.ForEach(e => e.GetComponentInChildren<T>().enabled = value);
    }

    private void SetupNextStateKillList(List<GameObject> enemiesToKill)
    {
        NextStateKillList = enemiesToKill.Select(e => e.GetComponentInChildren<IDamageable>()).ToList();
        NextStateKillList.ForEach(k =>
        {
            k.enabled = true;
            k.Died += onCombatCompleted;
        });
    }

    private void PlayCrashAudio()
    {
        var audioSource = GetFreeAudioSource();
        audioSource.clip = beaconLandAudio;
        audioSource.volume = 1;
        audioSource.Play();
        StartCoroutine(FreeAudioSource(beaconLandAudio.length, audioSource));
    }

    #endregion
}
