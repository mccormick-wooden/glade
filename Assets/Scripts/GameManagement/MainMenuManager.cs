public class MainMenuManager : BaseSceneManager
{
    public override string ManagedSceneName => "MainMenu";

    public override GameState ManagedState => GameState.MainMenu;

    protected override void OnSceneLoaded()
    {
    }

    protected override void OnSceneUnloaded()
    {
    }

    /// <summary>
    /// Intended to be fired primarily by a button.
    /// </summary>
    public void StartNewGame()
    {
        GameManager.UpdateGameState(GameState.NewGame);
    }
}
