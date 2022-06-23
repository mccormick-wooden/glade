using System;
using UnityEngine;

[RequireComponent(typeof(AnimationEventDispatcher))]
public class NewGameStateManager : BaseStateManager
{
    /// <summary>
    /// Controls whether the crawl will be skipped.
    /// This is purely for dev / debug QoL - eventually a button will be added for the player to do this (TODO:).
    /// </summary>
    [SerializeField]
    private bool skipCrawl = false; // Set to true to skip crawl

    /// <summary>
    /// Controls the speed of the crawl using timeScale.
    /// </summary>
    [SerializeField]
    private float timeScale = 1;

    private AnimationEventDispatcher animationEventDispatcher;

    protected void FixedUpdate()
    {
#if UNITY_EDITOR
        if (skipCrawl)
            GameManager.UpdateGameState(GameState.Level1);

        Time.timeScale = timeScale;
#endif
    }
    protected override void OnSceneLoaded()
    {
        animationEventDispatcher = GameObject.Find("CrawlText").GetComponent<AnimationEventDispatcher>();
        if (animationEventDispatcher == null)
            Debug.LogError($"{GetType().Name}: {nameof(animationEventDispatcher)} is null.");

        animationEventDispatcher.OnAnimationComplete += OnAnimationComplete;
    }

    protected override void OnSceneUnloaded()
    {
        animationEventDispatcher.OnAnimationComplete -= OnAnimationComplete;
        Time.timeScale = 1;
    }

    /// <summary>
    /// Callback to handle completion of crawl animation event.
    /// </summary>
    /// <param name="animation"></param>
    private void OnAnimationComplete(string animation)
    {
        if (animation.Equals("NewGameCrawl", StringComparison.OrdinalIgnoreCase))
            GameManager.UpdateGameState(GameState.Level1);
    }
}
