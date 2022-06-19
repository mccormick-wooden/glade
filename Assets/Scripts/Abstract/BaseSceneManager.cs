using System;
using UnityEngine;

public abstract class BaseSceneManager : MonoBehaviour
{
    public abstract string ManagedSceneName { get; }

    public abstract GameState ManagedState { get; }

    private DateTime lastLog = DateTime.Now;

    protected virtual void Awake()
    {
        if (!SceneLoader.CanLoadScene(ManagedSceneName))
        {
            throw new ArgumentOutOfRangeException($"{GetType().Name}.{nameof(ManagedSceneName)}",
                ManagedSceneName,
                $"Scene '{ManagedSceneName}' can't be loaded. Ensure scene exists and is configured in build settings!");
        }

        enabled = false;

        GameManager.OnStateChanged += GameManagerOnStateChanged;

        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        GameManager.OnStateChanged -= GameManagerOnStateChanged;
    }

    protected virtual void Update()
    {
        if (DateTime.Now > lastLog.AddSeconds(1))
        {
            Debug.Log($"Updating {GetType().Name}");
            lastLog = DateTime.Now;
        }
    }

    protected virtual void GameManagerOnStateChanged(GameState newState)
    {
        //if (newState == managedState && !ManagedSceneIsActive)
        //    SceneLoader.LoadScene(managedSceneName);
        if (newState == ManagedState && !enabled)
        {
            enabled = true;
            Debug.Log($"event received by {GetType().Name}: turning ON");
        }
        else if (newState != ManagedState && enabled)
        {
            enabled = false;
            Debug.Log($"event received by {GetType().Name}: turning OFF");
        }
    }
}
