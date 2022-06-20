public class MainMenuManager : BaseSceneManager
{
    public override string ManagedSceneName => "MainMenu";

    public override GameState ManagedState => GameState.MainMenu;

    /// <summary>
    /// Intended to be fired primarily by a button.
    /// </summary>
    public void StartNewGame()
    {
        GameManager.UpdateGameState(GameState.NewGame);
    }
}
