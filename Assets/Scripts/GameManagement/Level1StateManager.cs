
/// <summary>
/// We might not need derived level managers, but keeping this for now.
/// </summary>
public class Level1StateManager : BaseLevelStateManager
{
    protected override void UpdateNextGameState()
    {
        GameManager.instance.UpdateGameState(GameState.MainMenu);
    }
}
