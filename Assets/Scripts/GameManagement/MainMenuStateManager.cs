using UnityEngine;
using UnityEngine.Events;
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
        void AddMainMenuButtonCallback(string buttonRootName, UnityAction method)
        {
            var buttonRoot = GameObject.Find(buttonRootName);
            if (buttonRoot == null)
                Debug.LogError($"{GetType().Name}: {nameof(buttonRoot)} '{buttonRootName}' is null.");

            var button = buttonRoot.GetComponentInChildren<Button>();
            if (button == null)
                Debug.LogError($"{GetType().Name}: {nameof(button)} is null.");

            button.onClick.AddListener(method);
        }

        AddMainMenuButtonCallback(mainNewGameRootName, () => GameManager.UpdateGameState(GameState.NewGame));
        AddMainMenuButtonCallback(mainExitRootName, () => Quitter.QuitGame());
    }

    protected override void OnSceneUnloaded()
    {
    }
}
