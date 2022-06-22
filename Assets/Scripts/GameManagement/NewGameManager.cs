using System;
using UnityEngine;

public class NewGameManager : BaseSceneManager
{
    [SerializeField]
    private bool skipCrawl = false;

    [SerializeField]
    private float speedUpScale = 1;

    public override GameState ManagedState => GameState.NewGame;

    private AnimationEventDispatcher animationEventDispatcher;

    protected override void OnSceneLoaded()
    {
        if (skipCrawl)
        {
            GameManager.UpdateGameState(GameState.Level1);
            return;
        }
        else
        {
            animationEventDispatcher = GameObject.Find("CrawlText")?.GetComponent<AnimationEventDispatcher>();
            animationEventDispatcher.OnAnimationComplete += OnAnimationComplete;
            Time.timeScale = speedUpScale;
        }
    }

    protected override void OnSceneUnloaded()
    {
        animationEventDispatcher.OnAnimationComplete -= OnAnimationComplete;
        Time.timeScale = 1;
    }

    // TODO: Add option to skip Crawl
    private void OnAnimationComplete(string animation)
    {
        if (animation.Equals("NewGameCrawl", StringComparison.OrdinalIgnoreCase))
            GameManager.UpdateGameState(GameState.Level1);
    }
}
