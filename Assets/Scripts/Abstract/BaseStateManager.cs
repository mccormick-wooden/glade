using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BaseStateManager : MonoBehaviour
{
    /// <summary>
    /// The state owned by this manager. 
    /// Separation of managedState / managedSceneName allows us to easily swap out test scenes for a certain state.
    /// </summary>
    [Header("Game Settings")]
    [SerializeField]
    protected GameState managedState;

    /// <summary>
    /// The scene that this manager will manage. 
    /// Scene obviously must exist, but it also must be included in build settings.
    /// </summary>
    [SerializeField]
    protected string managedSceneName;

    public GameState ManagedState => managedState;

    public string ManagedSceneName => managedSceneName;

    public bool ManagedSceneIsActive => SceneLoader.GetCurrentSceneName() == ManagedSceneName;

    /// <summary>
    /// Awake has the following purposes:
    /// - Ensure that the state being managed is set and valid.
    /// - Ensure that the scene this is managing actually exists.
    /// - Start off in a disabled state (only enable if GameManager tells us to)
    /// - Subscribe to GameManager and SceneManager updates so we can answer the Call of Duty
    /// 
    /// Derived classes should put startup logic in Start() if absolutely needed, but they probably shouldn't need to.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected void Awake()
    {
        if (ManagedState == GameState.Invalid)
        {
            throw new ArgumentOutOfRangeException($"{GetType().Name}.{nameof(ManagedState)}",
                ManagedState,
                $"Must select a managed state.");
        }

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
    /// We use OnDestroy to unsubscribe from event(s) to prevent memory leaks. 
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
        if (debugOutput)
            Debug.Log($"State change '{newState}' received by {GetType().Name}.");

        if (newState == ManagedState && !ManagedSceneIsActive)
            SceneLoader.LoadScene(ManagedSceneName);
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
        if (!enabled && scene.name == ManagedSceneName)
        {
            enabled = true;
            OnSceneLoaded();

            if (debugOutput)
                Debug.Log($"{GetType().Name}: ENABLED, {ManagedSceneName}: LOADED");
        }
    }

    /// <summary>
    /// Derived class implementation for OnSceneLoaded even.
    /// </summary>
    protected abstract void OnSceneLoaded();

    /// <summary>
    /// Scene GameObjects exist until this event wraps up, so cleanup can be performed here.
    /// Implement specific derived behavior in OnSceneUnloaded.
    /// </summary>
    /// <param name="scene"></param>
    private void SceneManagerOnSceneUnloaded(Scene scene)
    {
        if (enabled && scene.name == ManagedSceneName)
        {
            enabled = false;
            OnSceneUnloaded();

            if (debugOutput)
                Debug.Log($"{GetType().Name}: DISABLED, {ManagedSceneName}: UNLOADED");
        }
    }

    /// <summary>
    /// Derived class implementation for OnSceneUnloaded event.
    /// </summary>
    protected abstract void OnSceneUnloaded();

    #region debug

    /// <summary>
    /// Controls all SceneManager debug output.
    /// </summary>
    [Header("Debug Settings")]
    [SerializeField]
    protected bool debugOutput = false;

    /// <summary>
    /// The rate in seconds at which heartbeat debug outputs are generated.
    /// A 0 value means no heartbeat output will be generated.
    /// Higher values are useful to reduce noise when other types of debug output are more important.
    /// </summary>
    [SerializeField]
    protected float heartbeatRate = 0f;

    private void OnEnable()
    {
        if (debugOutput && heartbeatRate > 0)
            InvokeRepeating("Heartbeat", 1f, heartbeatRate);
    }

    private void OnDisable()
    {
        if (debugOutput && heartbeatRate > 0)
            CancelInvoke("Heartbeat");
    }

    protected virtual void Heartbeat()
    {
        Debug.Log($"{GetType().Name} is alive");
    }
    #endregion
}
