public class MainMenuManager : BaseSceneManager
{
    public override string ManagedSceneName => "MainMenu";

    public override GameState ManagedState => GameState.MainMenu;

    public void StartNewGame()
    {
        GameManager.UpdateGameState(GameState.NewGame);
    }
}
