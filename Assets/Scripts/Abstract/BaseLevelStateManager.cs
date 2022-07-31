using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Interfaces;
using Beacons;
using PowerUps;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseLevelStateManager : BaseStateManager
{
    /// <summary>
    /// The playerGameObjectRootName must be the root of whatever object the player model lives on.
    /// </summary>
    [Header("Scene Settings")]
    [SerializeField]
    protected string playerGameObjectRootName = "Player";

    /// <summary>
    /// The beaconSpawnerObjectRootName must be the root of whatever object the beacon spawner lives on.
    /// </summary>
    [SerializeField]
    protected string beaconParentObjectRootName = "BeaconParent";

    /// <summary>
    /// When the game ends, the user is sent to the main menu.
    /// This field determines how many seconds it takes for this action to occur automatically. 
    /// </summary>
    [SerializeField]
    protected int returnToMainMenuCountdownLength = 10;
    protected int returnToMainMenuTimer;

    protected GameObject player;
    protected IDamageable playerDamageModel;
    protected TextMeshProUGUI HUDMessageText;

    protected GameObject beaconParent;
    protected BeaconSpawner beaconSpawner;

    protected EnemySpawner enemySpawner;

    protected GladeHealthManager gladeHealthManager;

    private bool stateEnding = false;

    protected override void OnSceneLoaded()
    {
        returnToMainMenuTimer = returnToMainMenuCountdownLength;

        #region player objs and subscriptions

        player = GameObject.Find(playerGameObjectRootName);
        Utility.LogErrorIfNull(player, nameof(player));

        playerDamageModel = player.GetComponentInChildren<IDamageable>();
        Utility.LogErrorIfNull(playerDamageModel, nameof(playerDamageModel));

        HUDMessageText = player.GetComponentInChildren<TextMeshProUGUI>();
        Utility.LogErrorIfNull(HUDMessageText, nameof(HUDMessageText));

        enemySpawner = player.GetComponentInChildren<EnemySpawner>();
        Utility.LogErrorIfNull(enemySpawner, nameof(enemySpawner));

        playerDamageModel.Died += OnPlayerDied;
        enemySpawner.EnemySpawned += OnEnemySpawned;

        #endregion

        #region beacon objs and subscriptions

        beaconParent = GameObject.Find(beaconParentObjectRootName);
        Utility.LogErrorIfNull(beaconParent, nameof(beaconParent));

        beaconSpawner = beaconParent.GetComponentInChildren<BeaconSpawner>();
        Utility.LogErrorIfNull(beaconSpawner, nameof(beaconSpawner));

        beaconSpawner.BeaconDied += OnBeaconDied;
        beaconSpawner.AllBeaconsDied += OnAllBeaconsDied;

        #endregion

        #region glade health objs and subscriptions

        gladeHealthManager = GameObject.Find("GladeHealthManager").GetComponent<GladeHealthManager>();
        Utility.LogErrorIfNull(gladeHealthManager, nameof(gladeHealthManager));

        gladeHealthManager.GladeDied += OnGladeDied;

        #endregion
    }

    protected override void OnSceneUnloaded()
    {
        playerDamageModel.Died -= OnPlayerDied;
        beaconSpawner.BeaconDied -= OnBeaconDied;
        beaconSpawner.AllBeaconsDied -= OnAllBeaconsDied;
        enemySpawner.EnemySpawned -= OnEnemySpawned;
        gladeHealthManager.GladeDied -= OnGladeDied;

        CancelInvoke(); // Clean up any active invokes.
    }

    /// <summary>
    /// Callback that handles the BeaconSpawner.AllBeaconsDied event.
    /// This is effectively the "game won" callback.
    /// </summary>
    private void OnAllBeaconsDied()
    {
        endGameMenu.SetText($"YOU WON!\nThe invasion is over!");
        endGameMenu.SetActive(true);
    }

    /// <summary>
    /// Callback that handles the IDamageable.Died event for the player model.
    /// </summary>
    /// <param name="damageModel">A reference to the object emitting the event</param>
    /// <param name="name">Name of the GameObject that IDamageable is attached to.</param>
    /// <param name="instanceId">Unity InstanceId of the GameObject that IDamageable is attached to</param>
    private void OnPlayerDied(IDamageable damageModel, string name, int instanceId)
    {
        endGameMenu.SetText($"Ya died, ya dingus.\nRestart, if you're not a quitter!");
        endGameMenu.SetActive(true);
    }

    /// <summary>
    /// Callback that handles the GladeDied event for the GladeHealthManager.
    /// </summary>
    private void OnGladeDied()
    {

        endGameMenu.SetText($"You FOOLISH CHILD! You let the Glade DIE!\nRestart, if you're not a quitter!");
        endGameMenu.SetActive(true);
    }

    /// <summary>
    /// Passes info to GladeHealthManager about active enemies in level
    /// </summary>
    /// <param name="damageModel"></param>
    private void OnEnemySpawned(IDamageable damageModel)
    {
        damageModel.Died += OnEnemyDied;
        gladeHealthManager.OnEnemySpawned(damageModel);
    }

    /// <summary>
    /// Passes info to GladeHealthManager about active enemies in level
    /// </summary>
    /// <param name="damageModel"></param>
    private void OnEnemyDied(IDamageable damageModel, string name, int instanceId)
    {
        damageModel.Died -= OnEnemyDied;
        gladeHealthManager.OnEnemyDied(damageModel);
    }

    /// <summary>
    /// Passes info to GladeHealthManager about beacon deaths
    /// </summary>
    private void OnBeaconDied()
    {
        gladeHealthManager.BeaconDied();
    }
}
