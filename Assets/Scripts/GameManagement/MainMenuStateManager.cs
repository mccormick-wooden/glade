using UnityEngine;
using UnityEngine.UI;

public class MainMenuStateManager : BaseStateManager
{
    [Header("Scene Settings")]
    [SerializeField]
    private string mainNewGameRootName = "MainNewGame";

    [SerializeField]
    private string mainExitRootName = "MainExitGame";

    protected override void OnSceneLoaded()
    {
        Utility.AddButtonCallback(mainNewGameRootName, () => GameManager.instance.UpdateGameState(GameState.NewGame));
        Utility.AddButtonCallback(mainExitRootName, () => Quitter.QuitGame());
        GameObject.Find(mainNewGameRootName).GetComponentInChildren<Button>().Select();
    }

    protected override void OnSceneUnloaded()
    {
    }
}
