
using Assets.Scripts.Abstract;
using UnityEngine;

public abstract class BaseLevelSceneManager : BaseSceneManager
{
    protected GameObject player;
    protected BaseDamageable playerDamageModel;

    protected override void OnSceneLoaded()
    {
        player = GameObject.Find("ybot");
        if (player == null)
            Debug.LogError($"{GetType().Name}: {nameof(player)} is null.");

        playerDamageModel = player.GetComponent<BaseDamageable>();
        if (playerDamageModel == null)
            Debug.LogError($"{GetType().Name}: {nameof(playerDamageModel)} is null.");

        playerDamageModel.Died += OnPlayerDied;
    }

    protected override void OnSceneUnloaded()
    {
        playerDamageModel.Died -= OnPlayerDied;
    }

    protected virtual void OnPlayerDied(string name, int instanceId)
    {
        Debug.Log($"GameObj '{name}:{instanceId}' died.");
    }

    /// <summary>
    /// SHOULD BE DELETED 
    /// </summary>
    protected override void Heartbeat()
    {
        base.Heartbeat();

        playerDamageModel.testdamage(20);
    }
}

