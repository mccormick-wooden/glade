using Assets.Scripts.Abstract;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BaseDamageable), typeof(TextMeshProUGUI))]
public abstract class BaseLevelSceneManager : BaseSceneManager
{
    /// <summary>
    /// The playerGameObjectRootName must be the root of whatever object the player model lives on, or nothing will work.
    /// </summary>
    [Header("Level Settings")]
    [SerializeField]
    protected string playerGameObjectRootName = "Player";

    /// <summary>
    /// When the game ends, the user is sent to the main menu.
    /// This field determines how many seconds it takes for this action to occur automatically. 
    /// </summary>
    [SerializeField]
    protected int returnToMainMenuCountdownLength = 10; 

    protected GameObject player;
    protected BaseDamageable playerDamageModel;
    protected TextMeshProUGUI HUDMessageText;

    protected override void OnSceneLoaded()
    {
        player = GameObject.Find(playerGameObjectRootName);
        if (player == null)
            Debug.LogError($"{GetType().Name}: {nameof(player)} is null.");

        playerDamageModel = player.GetComponentInChildren<BaseDamageable>();
        if (playerDamageModel == null)
            Debug.LogError($"{GetType().Name}: {nameof(playerDamageModel)} is null.");

        HUDMessageText = player.GetComponentInChildren<TextMeshProUGUI>();
        if (HUDMessageText == null)
            Debug.LogError($"{GetType().Name}: {nameof(HUDMessageText)} is null.");


        playerDamageModel.Died += OnPlayerDied;
    }

    protected override void OnSceneUnloaded()
    {
        playerDamageModel.Died -= OnPlayerDied;
        CancelInvoke();
    }

    /// <summary>
    /// Main callback that handles the BaseDamageable.Died event for the player model.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="instanceId"></param>
    private void OnPlayerDied(string name, int instanceId)
    {
        if (debugOutput)
            Debug.Log($"GameObj '{name}:{instanceId}' died.");

        InvokeRepeating("OnPlayerDiedReturnToMainMenuCountdown", 0f, 1f);
    }

    private void OnPlayerDiedReturnToMainMenuCountdown()
    {
        HUDMessageText.fontSize = 50;
        HUDMessageText.text = $"Ya died, ya dingus.\n\nReturning to Main Menu in {returnToMainMenuCountdownLength} seconds...";
        returnToMainMenuCountdownLength -= 1;
        if (returnToMainMenuCountdownLength < 0)
            ReturnToMainMenu();
    }

    /// <summary>
    /// TODO: Add a button to the canvas wired to this method that allows the player to return to menu early.
    /// </summary>
    private void ReturnToMainMenu()
    {
        GameManager.UpdateGameState(GameState.MainMenu);
    }
}

