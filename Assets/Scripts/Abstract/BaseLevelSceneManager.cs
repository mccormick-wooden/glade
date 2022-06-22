
using Assets.Scripts.Abstract;
using TMPro;
using UnityEngine;

public abstract class BaseLevelSceneManager : BaseSceneManager
{
    protected GameObject player;
    protected GameObject playerModel;
    protected BaseDamageable playerDamageModel;
    protected GameObject HUD;
    protected GameObject HUDMessage;
    protected TextMeshProUGUI HUDMessageText;

    protected override void OnSceneLoaded()
    {
        player = GameObject.Find("Player");
        if (player == null)
            Debug.LogError($"{GetType().Name}: {nameof(player)} is null.");

        playerDamageModel = player.GetComponentInChildren<BaseDamageable>();
        if (playerDamageModel == null)
            Debug.LogError($"{GetType().Name}: {nameof(playerDamageModel)} is null.");

        //playerModel = player.transform.GetChild(0).gameObject;
        //if (playerModel == null)
        //    Debug.LogError($"{GetType().Name}: {nameof(playerModel)} is null.");

        //playerDamageModel = playerModel.GetComponent<BaseDamageable>();
        //if (playerDamageModel == null)
        //    Debug.LogError($"{GetType().Name}: {nameof(playerDamageModel)} is null.");

        //HUD = player.transform.GetChild(1).gameObject;
        //if (HUD == null)
        //    Debug.LogError($"{GetType().Name}: {nameof(HUD)} is null.");

        //HUDMessage = HUD.transform.GetChild(0).gameObject;
        //if (HUDMessage == null)
        //    Debug.LogError($"{GetType().Name}: {nameof(HUDMessage)} is null.");

        //HUDMessageText = HUDMessage.GetComponent<TextMeshProUGUI>();
        ////if (HUDMessageText == null)
        //    //Debug.LogError($"{GetType().Name}: {nameof(HUDMessageText)} is null.");

        ////HUDMessageText.text = "yay";

        playerDamageModel.Died += OnPlayerDied;
    }

    protected override void OnSceneUnloaded()
    {
        playerDamageModel.Died -= OnPlayerDied;
    }

    protected virtual void OnPlayerDied(string name, int instanceId)
    {
        Debug.Log($"GameObj '{name}:{instanceId}' died.");
        // TODO: end game
    }
}

