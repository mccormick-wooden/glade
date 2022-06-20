using System;
using UnityEngine;

public abstract class BaseSceneManager : MonoBehaviour
{
    public abstract string ManagedSceneName { get; }

    public abstract GameState ManagedState { get; }

    public bool ManagedSceneIsActive => SceneLoader.GetCurrentSceneName() == ManagedSceneName;

    private DateTime lastHeartbeat = DateTime.Now;

    /// <summary>
    /// Awake has the following purposes:
    /// - Ensure that the scene this is managing actually exists.
    /// - Start off in a disabled state (only enable if GameManager tells us to)
    /// - Subscribe to GameManager updates so we can answer the Call of Duty
    /// - Ensure that this is included in the set of GameObjects that are globally preserved (DontDestroyOnLoad
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

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Unsubscribe from event(s) to prevent memory leaks. 
    /// </summary>
    protected void OnDestroy()
    {
        GameManager.OnStateChanged -= GameManagerOnStateChanged;
    }

    /// <summary>
    /// Derived classes should probably implement Update().
    /// Base implementation should be called.
    /// </summary>
    protected virtual void Update()
    {
        CheckHeartbeat();
    }

    /// <summary>
    /// Main callback that handles GameManager updates.
    /// 
    /// Derived classes should be enable/disable behavior in OnEnable() / OnDisable() when possible
    /// Alternatively, enable/disable can be implemented in Update() and cached.
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
    /// Purely for dev / logging
    /// </summary>
    private void CheckHeartbeat()
    {
        if (DateTime.Now > lastHeartbeat.AddSeconds(1))
        {
            Debug.Log($"{GetType().Name} is alive");
            lastHeartbeat = DateTime.Now;
        }
    }
}
