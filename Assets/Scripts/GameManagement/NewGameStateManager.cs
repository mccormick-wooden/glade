using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AnimationEventDispatcher))]
public class NewGameStateManager : BaseStateManager
{
    /// <summary>
    /// Controls whether the crawl will be skipped.
    /// This is purely for dev / debug QoL - eventually a button will be added for the player to do this (TODO:).
    /// </summary>
    [Header("Scene Settings")]
    [SerializeField]
    private bool skipCrawl = false; // Set to true to skip crawl

    /// <summary>
    /// Controls the speed of the crawl using timeScale.
    /// </summary>
    [SerializeField]
    private float timeScale = 1;

    private AnimationEventDispatcher animationEventDispatcher;
    private LongClickButton sceneSkipperButton;

    protected void FixedUpdate()
    {
#if UNITY_EDITOR
        if (skipCrawl)
        {
            skipCrawl = false;
            UpdateNextGameState();
        }

        Time.timeScale = timeScale;
#endif
    }

    protected override void OnSceneLoaded()
    {
        animationEventDispatcher = GameObject.Find("CrawlText").GetComponent<AnimationEventDispatcher>();
        Utility.LogErrorIfNull(animationEventDispatcher, nameof(animationEventDispatcher));

        sceneSkipperButton = GameObject.Find("SceneSkipperButton").GetComponent<LongClickButton>();
        Utility.LogErrorIfNull(sceneSkipperButton, nameof(sceneSkipperButton));

        animationEventDispatcher.OnAnimationComplete += OnAnimationComplete;
        sceneSkipperButton.LongClickComplete += UpdateNextGameState;
    }

    protected override void OnSceneUnloaded()
    {
        animationEventDispatcher.OnAnimationComplete -= OnAnimationComplete;
        sceneSkipperButton.LongClickComplete -= UpdateNextGameState;
        Time.timeScale = 1;
    }

    /// <summary>
    /// Callback to handle completion of crawl animation event.
    /// </summary>
    /// <param name="animation"></param>
    private void OnAnimationComplete(string animation)
    {
        if (animation.Equals("NewGameCrawl", StringComparison.OrdinalIgnoreCase))
            UpdateNextGameState();
    }

    protected override void UpdateNextGameState()
    {
        GameManager.instance.UpdateGameState(GameState.Training);
    }
}
