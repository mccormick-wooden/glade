using System;
using UnityEngine;

public class NewGameManager : BaseSceneManager
{
    public override string ManagedSceneName => "NewGame";

    public override GameState ManagedState => GameState.NewGame;

    private AnimationEventDispatcher animationEventDispatcher;

    private bool subscribed = false;

    protected override void Update()
    {
        base.Update();

        // I would strongly prefer to do this logic in OnEnable(), but it's pretty racy... must be doing something wrong, but shouldn't matter too much?
        if (animationEventDispatcher == null)
        {
            animationEventDispatcher = GameObject.Find("CrawlText")?.GetComponent<AnimationEventDispatcher>();
        }
        else if (!subscribed)
        {
            animationEventDispatcher.OnAnimationComplete += OnAnimationComplete;
            subscribed = true;
        }
    }

    private void OnDisable()
    {
        if (animationEventDispatcher != null)
            animationEventDispatcher.OnAnimationComplete -= OnAnimationComplete;
    }

    private void OnAnimationComplete(string animation)
    {
        Debug.Log($"{GetType().Name} received AnimationComplete for {animation}!");
        if (animation.Equals("NewGameCrawl", StringComparison.OrdinalIgnoreCase))
            GameManager.UpdateGameState(GameState.MainMenu);
    }
}
