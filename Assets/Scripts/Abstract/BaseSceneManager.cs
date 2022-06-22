using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BaseSceneManager : MonoBehaviour
{
    public abstract string ManagedSceneName { get; }

    public abstract GameState ManagedState { get; }

    public bool ManagedSceneIsActive => SceneLoader.GetCurrentSceneName() == ManagedSceneName;

    /// <summary>
    /// Awake has the following purposes:
    /// - Ensure that the scene this is managing actually exists.
    /// - Start off in a disabled state (only enable if GameManager tells us to)
    /// - Subscribe to GameManager and SceneManager updates so we can answer the Call of Duty
    /// 
    /// Derived classes should put startup logic in Start()
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected void Awake()
    {
        if (!SceneLoader.CanLoadScene(ManagedSceneName))
        {
            throw new ArgumentOutOfRangeException($"{GetType().Name}.{nameof(ManagedSceneName)}",
                ManagedSceneName,
                $"Scene '{ManagedSceneName}' can't be loaded. Ensure scene exists and is configured in build settings!");
        }

        enabled = false;

        GameManager.OnStateChanged += GameManagerOnStateChanged;
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;

    }

    /// <summary>
    /// Unsubscribe from event(s) to prevent memory leaks. 
    /// </summary>
    protected void OnDestroy()
    {
        GameManager.OnStateChanged -= GameManagerOnStateChanged;
        SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
        SceneManager.sceneUnloaded -= SceneManagerOnSceneUnloaded;
    }

    /// <summary>
    /// Main callback that handles GameManager updates.
    /// 
    /// Derived classes should implement specific enable/disable behavior in OnSceneLoaded()/OnSceneUnloaded() 
    /// because it is the only place that ensures GameObjects exist in scene context.
    /// </summary>
    /// <param name="newState"></param>
    protected void GameManagerOnStateChanged(GameState newState)
    {
        Debug.Log($"State change received by {GetType().Name}.");

        if (newState == ManagedState && !ManagedSceneIsActive)
            SceneLoader.LoadScene(ManagedSceneName);

        if (newState == ManagedState && !enabled)
        {
            enabled = true;
            Debug.Log($"{GetType().Name}: ENABLED");
        }
        else if (newState != ManagedState && enabled)
        {
            enabled = false;
            Debug.Log($"{GetType().Name}: DISABLED");
        }
    }

    /// <summary>
    /// Callback to handle scene load events.
    /// Since we can't get references to a scene's game objects until the scene is loaded, we need to listen for that event.
    /// Implement specific derived behavior in OnSceneLoaded.
    /// See https://answers.unity.com/questions/1174255/since-onlevelwasloaded-is-deprecated-in-540b15-wha.html
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="loadSceneMode"></param>
    private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (enabled && scene.name == ManagedSceneName)
        {
            Debug.Log($"{scene.name} is loaded!");
            OnSceneLoaded();
        }
    }

    protected abstract void OnSceneLoaded();

    /// <summary>
    /// Scene GameObjects exist until this event wraps up, so cleanup can be performed here.
    /// Implement specific derived behavior in OnSceneUnloaded.
    /// </summary>
    /// <param name="scene"></param>
    private void SceneManagerOnSceneUnloaded(Scene scene)
    {
        if (scene.name == ManagedSceneName)
        {
            Debug.Log($"{scene.name} is unloaded!");
            OnSceneUnloaded(); 
        }
    }

    protected abstract void OnSceneUnloaded();

    private void OnEnable()
    {
        InvokeRepeating("Heartbeat", 1f, 1f);
    }

    private void OnDisable()
    {
        CancelInvoke("Heartbeat");
    }

    /// <summary>
    /// Purely for dev / logging
    /// </summary>
    protected virtual void Heartbeat()
    {
        Debug.Log($"{GetType().Name} is alive");
    }
}
