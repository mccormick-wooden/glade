public class MainMenuManager : BaseSceneManager
{
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
