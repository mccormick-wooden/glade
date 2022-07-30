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

    protected Slider gladeHealthBar;

    protected enum GladeHealthState
    {
        High,
        Med,
        Low
    }

    protected Dictionary<GladeHealthState, Image> gladeHealthImageDict = new Dictionary<GladeHealthState, Image>();

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

        playerDamageModel.Died += OnPlayerDied;

        #endregion

        #region beacon objs and subscriptions

        beaconParent = GameObject.Find(beaconParentObjectRootName);
        Utility.LogErrorIfNull(beaconParent, nameof(beaconParent));

        beaconSpawner = beaconParent.GetComponentInChildren<BeaconSpawner>();
        Utility.LogErrorIfNull(beaconSpawner, nameof(beaconSpawner));

        beaconSpawner.AllBeaconsDied += OnAllBeaconsDied;

        #endregion

        #region gladehealth
        gladeHealthBar = GameObject.Find("HUDGladeHealthBar").GetComponent<Slider>();

        gladeHealthImageDict = new Dictionary<GladeHealthState, Image>
        {
            { GladeHealthState.High, GameObject.Find("HighGladeHealthFill").GetComponent<Image>() },
            { GladeHealthState.Med, GameObject.Find("MedGladeHealthFill").GetComponent<Image>() },
            { GladeHealthState.Low, GameObject.Find("LowGladeHealthFill").GetComponent<Image>() }
        };

        UpdateGladeHealthBar(100);
        #endregion
    }

    protected override void OnSceneUnloaded()
    {
        playerDamageModel.Died -= OnPlayerDied;
        beaconSpawner.AllBeaconsDied -= OnAllBeaconsDied;

        CancelInvoke(); // Clean up any active invokes.
    }

    /// <summary>
    /// Callback that handles the BeaconSpawner.AllBeaconsDied event.
    /// This is effectively the "game won" callback.
    /// </summary>
    private void OnAllBeaconsDied()
    {
        if (debugOutput)
            Debug.Log("All beacons died.");

        player.GetComponentInChildren<IDamageable>().IsImmune = true;
        DisablePickups();
        InvokeRepeating("OnAllBeaconsDiedReturnToMainMenuCountdown", 0f, 1f);
    }

    private void DisablePickups()
    {
        // Disable all pickups on the ground
        PowerUpPickup[] pickups = FindObjectsOfType<PowerUpPickup>();
        foreach (PowerUpPickup pickup in pickups)
        {
            pickup.PowerUpEnabled = false;
        }
        
        // Close power up menu if already open
        PowerUpMenu menu = FindObjectOfType<PowerUpMenu>(true); // Include the inactive menu
        menu.gameObject.SetActive(false);
    }

    private void OnAllBeaconsDiedReturnToMainMenuCountdown()
    {
        HUDMessageText.fontSize = 50;
        HUDMessageText.text = $"YOU WON!\n\nReturning to Main Menu in {returnToMainMenuTimer} seconds...";

        returnToMainMenuTimer -= 1;
        if (returnToMainMenuTimer < 0)
            ReturnToMainMenu();
    }

    /// <summary>
    /// Callback that handles the IDamageable.Died event for the player model.
    /// This is effectively the "game lost" callback.
    /// </summary>
    /// <param name="damageModel">A reference to the object emitting the event</param>
    /// <param name="name">Name of the GameObject that IDamageable is attached to.</param>
    /// <param name="instanceId">Unity InstanceId of the GameObject that IDamageable is attached to</param>
    private void OnPlayerDied(IDamageable damageModel, string name, int instanceId)
    {
        if (debugOutput)
            Debug.Log($"GameObj '{name}:{instanceId}' died.");

        DisablePickups();
        InvokeRepeating("OnPlayerDiedReturnToMainMenuCountdown", 0f, 1f);
    }

    private void OnPlayerDiedReturnToMainMenuCountdown()
    {
        HUDMessageText.fontSize = 50;
        HUDMessageText.text = $"Ya died, ya dingus.\n\nReturning to Main Menu in {returnToMainMenuTimer} seconds...";

        returnToMainMenuTimer -= 1;
        if (returnToMainMenuTimer < 0)
            ReturnToMainMenu();
    }

    /// <summary>
    /// TODO: Add a button to the canvas wired to this method that allows the player to return to menu early.
    /// </summary>
    private void ReturnToMainMenu()
    {
        CancelInvoke(); // YOLO
        UpdateNextGameState();
    }

    private void Respawn()
    {
        CancelInvoke();
        GameManager.instance.UpdateGameState(GameManager.instance.State);
    }

    protected void UpdateGladeHealthBar(float health)
    {
        GladeHealthState state;

        if (health > 66)
            state = GladeHealthState.High;
        else if (health > 33)
            state = GladeHealthState.Med;
        else
            state = GladeHealthState.Low;

        UpdateGladeHealthBarFillType(state);

        gladeHealthBar.value = health;
    }

    protected void UpdateGladeHealthBarFillType(GladeHealthState state)
    {
        foreach (var kvp in gladeHealthImageDict)
        {
            kvp.Value.enabled = kvp.Key == state;
        }

        gladeHealthBar.fillRect = gladeHealthImageDict[state].rectTransform;
    }
}

