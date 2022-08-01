using System;
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
        AppleDialogue = 53,
        EatApple = 54,
        PostEatAppleDialoge = 55,
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

    [SerializeField]
    private string appleTreeVirtualCameraName = "AppleTreeVirtualCamera";
    private CinemachineVirtualCamera appleTreeVirtualCamera;

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
    private Action<HealingApple> onEatAppleCompleted = null;

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

    // Glade health
    private GladeHealthManager gladeHealthManager;

    private Dictionary<TrainingState, List<string>> dialogueDictionary = new Dictionary<TrainingState, List<string>>
    {
        { TrainingState.GeneralIntroDialogue, new List<string>()
            {
                "Ah, Warden of The Glades! You've arrived just in time!",
                "You're looking... blue? Did you just come back from Blue Man Group tryouts? HA! Just kidding of course, Warden. We all know you're terrible at the drums.",
                "Anyway, these gosh darn aliens are just causing the BIGGEST ruckus!",
                "I really need you to do me a solid and clear them out of here. Couldya do that for me?",
                "It looks like you brought your SWORD... Good! Do you remember how to use it? I guess it has been awhile.",
                "When you see those pesky ALIENS, just run right up to 'em! Use the LEFT STICK or the W/A/S/D keys to move. Don't be shy!",
                "Then once you're close, make sure you use RB or LEFT CLICK (on your mouse) to swing that sword until the aliens are good and dead!",
                "When at range, you can also use your WIND ATTACK - use RT / R key to send a couple deadly twisters at those aliens from a safe distance!",
                "If you need a quick second to collect your thoughts, use START or the ESC key to pause! Mental health is important!",
                "Oh crap, ALIENS INCOMING!!!"
            }
        },
        { TrainingState.EnemyIntroDialogue, new List<string>()
            {
                "Yup, those are \"aliens\" alright (just go with it).",
                "The THREE-HEADED HELLION (on the right) and the FLYING FEASTER (on the left) are both gonna just straight up try to eat you. I mean just look at 'em.",
                "Be especially careful around the MUSHROOM BOI - those ones have an explosive spore attack, and it is VERY deadly!",
                "There are other types of aliens I've seen, too - but don't have intel for ya. Sorry! You're gonna have to figure those out on your own!",
                "One tip - you can lock on to enemies by clicking the LEFT STICK or F key, and cycle through enemies using LEFT/RIGHT DPAD or Q / E keys!",
                "Super helpful if you're getting swarmed and you're tryna drop 'em one at a time!",
                "One other thing to keep in mind - the more aliens that are alive, the faster the HEALTH OF THE GLADE will be depleted!",
                "See that green bar in the top left? That's the HEALTH OF THE GLADE! Don't let it get too low or all is lost!",
                "I can help ya out if it gets too low up here - but down there, you're on your own, Warden!"
            }
        },
        { TrainingState.CrystalIntroDialogue, new List<string>()
            {
                "These CRYSTALS are being brought by the aliens to help them colonize the Glades!",
                "They work by weaponizing Glade Energy at those that threaten the alien colonization areas!",
                "Try not to get too close if you can help it - unless you're trying to kill the sucker!",
                "Don't worry too much about damage, though - I'll heal you if your health gets too low! That's what Ancient Tree Spirit friends are for!"
            }
        },
        { TrainingState.BeaconIntroDialogue, new List<string>()
            {
                "Ah, a BEACON! These things are falling from the sky, and that's how the aliens are getting here I reckon.",
                "Until you kill the BEACON, the aliens will keep flooding the area. I don't get it either. How do they all fit in that thing? It's not even that big.",
                "Even worse, the BEACON has an OVERSHIELD that persists as long as there are nearby ALIENS and CRYSTALS! These guys really thought of everything!",
                "Anyway, the aliens will defend the BEACONS with their lives - be careful, Warden!",
                "I almost forgot to mention, killing BEACONS will help restore the HEALTH OF THE GLADE!",
                "Kill the aliens first to protect the HEALTH OF THE GLADE, and we'll deal with the CRYSTAL and BEACON next."
            }
        },
        { TrainingState.PostEnemyCombatDialogue, new List<string>()
            {
                "Wow, you schmacked those fools!",
                "I doubt those are the last aliens we'll see - we need to kill the BEACON to stop them, but that CRYSTAL needs to go first.",
                "Remember, the CRYSTAL can hurt you at range, so this might be a good opportunity to use your WIND ATTACK with RT / R key!",
                "Keep an eye on your blue MANA bar in the upper left, though! Your WIND ATTACK uses that up!",
                "Of course, if your MANA runs out, you can use your SWORD again too - remember, it's RB/Left Click to swing!",
                "But I'm sure you know that by now, otherwise we're probably in trouble....",
                "However you decide to do it, destroy the CRYSTAL now!"
            }
        },
        { TrainingState.PostCrystalCombatDialogue, new List<string>()
            {
                "Man, that CRYSTAL lightning bolt is pretty sick. I mean, uhh, good job!",
                "I didn't even realize it was possible to harness the Glade-ergy like that...",
                "I'll send some saplings later to pick up the CRYSTAL scraps for research - write that down, it'll be a major plot point in Glade 2.",
                "In the meantime, go on ahead and kill the heck out of that BEACON!"
            }
        },
        { TrainingState.PostBeaconCombatDialogue, new List<string>()
            {
               "Huzzah! Great work, Warden.",
               "You'll see a lot more BEACONS soon - killing them is how I (uhh, I mean we) win!",
               "They'll show up as BLUE on your minimap - I'll give you your minimap before I send you off to the frontlines, don't worry!",
               "Also, you might be wondering what that Green Thing is - just so happens, it's a present from me to you! Yep!",
               "Go on ahead and pick it up - it'll let you enhance your abilities, and the best part is you get to choose how!",
            }
        },
        { TrainingState.PostPowerUpDialogue, new List<string>()
            {
                "There's one more thing I wanna tell you about before sending you on your way..."
            }
        },
        { TrainingState.AppleDialogue, new List<string>()
            {
                "Sprinkled though out the Glade, you'll find special trees like this, upon which the glorious GIFTS OF THE GLADE grow!",
                "And NO, they are NOT apples. I am SO sick of explaining this to people. We are on an ALIEN PLANET. Ever heard of convergent evolution? Gosh.",
                "Anyway, use your sword on the tree to knock the GIFT free! Then once it drops, get close to it and use the A button / G key to eat it for a health boost!",
                "Whatever you do, just please don't accidentally kick the GIFT off the mesa. You'll have to restart the game. We didn't have time to fix that!",
                "Just eat the GIFT, and then we'll talk!"
            }
        },
        { TrainingState.PostEatAppleDialoge, new List<string>()
            {
                "There now, wasn't that a tasty treat?",
                "Dang, it looks like the invasion is really getting started down there. You ready to get going? Think 2 minutes of combat training was enough?",
                "Don't answer that. Anyway, I'll go ahead and teleport you down to the invasion site so you can start clapping more aliens.",
                "Help me Obi-Warden Kenobi! You're my only hope! Good luck!!!"
            }
         }
    };

    private void DisableVirtualCameras()
    {
        trainingHostVirtualCamera.enabled = false;
        enemyVirtualCamera.enabled = false;
        crystalVirtualCamera.enabled = false;
        beaconVirtualCamera.enabled = false;
        appleTreeVirtualCamera.enabled = false;
    }

    protected override void OnSceneLoaded()
    {
        #region get references
        gladeHealthManager = GameObject.Find("GladeHealthManager").GetComponent<GladeHealthManager>();
        Utility.LogErrorIfNull(gladeHealthManager, nameof(gladeHealthManager));

        trainingHostVirtualCamera = GameObject.Find(trainingHostVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(trainingHostVirtualCamera, nameof(trainingHostVirtualCamera));

        enemyVirtualCamera = GameObject.Find(enemyVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(enemyVirtualCamera, nameof(enemyVirtualCamera));

        crystalVirtualCamera = GameObject.Find(crystalVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(crystalVirtualCamera, nameof(crystalVirtualCamera));

        beaconVirtualCamera = GameObject.Find(beaconVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(beaconVirtualCamera, nameof(beaconVirtualCamera));

        appleTreeVirtualCamera = GameObject.Find(appleTreeVirtualCameraName)?.GetComponent<CinemachineVirtualCamera>();
        Utility.LogErrorIfNull(appleTreeVirtualCameraName, nameof(appleTreeVirtualCameraName));

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
        trainingStateChanged += OnTrainingStateChanged_AppleDialogueState;
        trainingStateChanged += OnTrainingStateChanged_EatAppleState;
        trainingStateChanged += OnTrainingStateChanged_PostEatAppleDialogueState;
        trainingStateChanged += OnTrainingStateChanged_End;
        #endregion

        #region set initial states
        currentTrainingState = TrainingState.Invalid;
        UpdateControlStateGracefully(enableControlState: false); // Don't allow control on scene start
        dialogueCanvas.enabled = false;
        playerCrystalDamageEffect.enabled = false;
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
        trainingStateChanged -= OnTrainingStateChanged_AppleDialogueState;
        trainingStateChanged -= OnTrainingStateChanged_EatAppleState;
        trainingStateChanged -= OnTrainingStateChanged_PostEatAppleDialogueState;
        trainingStateChanged -= OnTrainingStateChanged_End;
    }

    protected override void UpdateNextGameState()
    {
        GameManager.instance.UpdateGameState(GameState.Level1); // TODO: I'd like the tree spirit to open a portal that the warden can walk through to end the scene
    }

    private List<string> GetDialogueForState(TrainingState state)
    {
        try
        {
            return dialogueDictionary[state];
        }
        catch
        {
            return new List<string> { "missing dialogue for state" };
        }
    }

    #region training state callbacks
    private void OnTrainingStateChanged_IntroDialogueState(TrainingState trainingState)
    {
        if (!trainingState.Equals(TrainingState.GeneralIntroDialogue))
            return;

        List<string> dialogueList = GetDialogueForState(trainingState);

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

        List<string> dialogueList = GetDialogueForState(trainingState);

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

        List<string> dialogueList = GetDialogueForState(trainingState);

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

        List<string> dialogueList = GetDialogueForState(trainingState);

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

        List<string> dialogueList = GetDialogueForState(trainingState);

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

        DynamicEnemies.ForEach(e => gladeHealthManager.OnEnemySpawned(e.GetComponent<IDamageable>()));

        onCombatCompleted = (IDamageable damageable, string name, int instanceId) =>
        {
            damageable.Died -= onCombatCompleted;
            gladeHealthManager.OnEnemyDied(damageable);

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
                playerCrystalDamageEffect.enabled = false;
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

        List<string> dialogueList = GetDialogueForState(trainingState);

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
            appleTreeVirtualCamera.enabled = true;
            dialogueCanvas.enabled = false;
        };

        onNextStateCameraBlendComplete = (ICinemachineCamera camera) =>
        {
            if (camera.Name == appleTreeVirtualCameraName)
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

    private void OnTrainingStateChanged_AppleDialogueState(TrainingState trainingState)
    {
        if (!trainingState.Equals(TrainingState.AppleDialogue))
            return;

        List<string> dialogueList = GetDialogueForState(trainingState);

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
                NextTrainingState();
            }
        };

        dialogueController.AllDialogueCompleted += onAllDialogueCompleted;
        cameraBlendEventDispatcher.CameraBlendCompleted += onNextStateCameraBlendComplete;
    }

    private void OnTrainingStateChanged_EatAppleState(TrainingState trainingState)
    {
        if (!trainingState.Equals(TrainingState.EatApple))
            return;

        onEatAppleCompleted = (healingApple) =>
        {
            healingApple.AppleEaten -= onEatAppleCompleted;
            Invoke("NextTrainingState", postCombatWait);
        };

        GameObject.Find("tree_1 (2)").GetComponentInChildren<HealingApple>().AppleEaten += onEatAppleCompleted;
    }

    private void OnTrainingStateChanged_PostEatAppleDialogueState(TrainingState trainingState)
    {
        if (trainingState != TrainingState.PostEatAppleDialoge)
            return;

        List<string> dialogueList = GetDialogueForState(trainingState);

        //string specialDialogue = string.Empty;
        //switch (appliedPowerUp)
        //{
        //    case DamageIncreasePowerUp t1:
        //        specialDialogue = "Wow, Warden you look JACKED now! That damage increase should definitely help.";
        //        break;
        //    case DamageResistPowerUp t2:
        //        specialDialogue = "Ooooo damage resistance! Get all damage spongey and whatnot, I like it!";
        //        break;
        //    case MaxHealthPowerUp t3:
        //        specialDialogue = "Increasing health is a good choice! Your skin looks healthier already!";
        //        break;
        //    default:
        //        specialDialogue = "Hmmm... That's a weird choice! Whatever, your funeral!";
        //        break;
        //}

        //dialogueList.Insert(0, specialDialogue);

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
    private void OnOutOfBoundsPlaneTriggered(Collider collider)
    {
        //Debug.Log("OnOutOfBoundsPlaneTriggered");
        if (collider.gameObject == playerModel)
            GameManager.instance.InvokeTransition(midTransitionAction: () => playerModel.transform.position = playerModelStartingPos);
    }

    private bool inPlayableState = false;

    private string lockOnCameraName = "LockOnCamera";

    private void FixedUpdate()
    {
        if (inPlayableState != playerScript.ControlsEnabled)
            UpdateControlStateGracefully(inPlayableState);
    }

    private void OnBlendToCameraStarted_DisableControlState(ICinemachineCamera activeCamera)
    {
        inPlayableState = activeCamera.Name == lockOnCameraName;
    }

    private void OnBlendToCameraCompleted_EnableControlState(ICinemachineCamera activeCamera)
    {
        inPlayableState = activeCamera.Name == playerCameraName || activeCamera.Name == lockOnCameraName;
    }
    #endregion

    #region helpers
    private void NextTrainingState()
    {
        playerCrystalDamageEffect.enabled = true;
        trainingStateChanged.Invoke(currentTrainingState = currentTrainingState.Next());

        Debug.Log($"TrainingStateChanged To: {currentTrainingState}");
    }

    private void StartDialogueState(CinemachineVirtualCamera dialogueCamera)
    {
        dialogueCamera.enabled = true;
        playerCrystalDamageEffect.enabled = false;
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
