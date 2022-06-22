using System;
using UnityEngine;

public class NewGameManager : BaseSceneManager
{
    public override string ManagedSceneName => "NewGame";

    public override GameState ManagedState => GameState.NewGame;

    private AnimationEventDispatcher animationEventDispatcher;

    protected override void OnSceneLoaded()
    {
        animationEventDispatcher = GameObject.Find("CrawlText")?.GetComponent<AnimationEventDispatcher>();
        animationEventDispatcher.OnAnimationComplete += OnAnimationComplete;
    }

    protected override void OnSceneUnloaded()
    {
        animationEventDispatcher.OnAnimationComplete -= OnAnimationComplete;
    }

    // TODO: Add option to skip Crawl
    private void OnAnimationComplete(string animation)
    {
        if (animation.Equals("NewGameCrawl", StringComparison.OrdinalIgnoreCase))
            GameManager.UpdateGameState(GameState.Level1);
    }
}
