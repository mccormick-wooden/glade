using UnityEngine;

public class MainMenuStateManager : BaseStateManager
{
    [Header("Scene Settings")]
    [SerializeField]
    private string mainNewGameRootName = "MainNewGame";

    [SerializeField]
    private string mainExitRootName = "MainExitGame";

    protected override void OnSceneLoaded()
    {
        Utility.AddButtonCallback(mainNewGameRootName, () => GameManager.UpdateGameState(GameState.NewGame));
        Utility.AddButtonCallback(mainExitRootName, () => Quitter.QuitGame());
    }

    protected override void OnSceneUnloaded()
    {
    }
}
