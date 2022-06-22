using System;
using UnityEngine;

public class NewGameManager : BaseSceneManager
{
    [SerializeField]
    private bool skipCrawl = false;

    [SerializeField]
    private float timeScale = 1;

    private AnimationEventDispatcher animationEventDispatcher;

    protected override void OnSceneLoaded()
    {
        animationEventDispatcher = GameObject.Find("CrawlText")?.GetComponent<AnimationEventDispatcher>();
        animationEventDispatcher.OnAnimationComplete += OnAnimationComplete;

        if (skipCrawl)
        {
            GameManager.UpdateGameState(GameState.Level1);
            return;
        }
        else
        {
            Time.timeScale = timeScale;
        }
    }

    protected override void OnSceneUnloaded()
    {
        animationEventDispatcher.OnAnimationComplete -= OnAnimationComplete;
        Time.timeScale = 1;
    }

    // TODO: Add option for player to skip Crawl 
    private void OnAnimationComplete(string animation)
    {
        if (animation.Equals("NewGameCrawl", StringComparison.OrdinalIgnoreCase))
            GameManager.UpdateGameState(GameState.Level1);
    }
}
