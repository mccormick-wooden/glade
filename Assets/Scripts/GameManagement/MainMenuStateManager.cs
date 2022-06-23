using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuStateManager : BaseStateManager
{
    [Header("Scene Settings")]
    [SerializeField]
    private string newGameButtonRootName = "NewGame";

    [SerializeField]
    private string exitGameButtonRootName = "ExitGame";

    private List<Button> managedButtons = new List<Button>();

    protected override void OnSceneLoaded()
    {
        void AddButtonListener(string buttonRootName, UnityAction method)
        {
            var buttonRoot = GameObject.Find(buttonRootName);
            if (buttonRoot == null)
                Debug.LogError($"{GetType().Name}: {nameof(buttonRoot)} '{buttonRootName}' is null.");

            var button = buttonRoot.GetComponentInChildren<Button>();
            if (button == null)
                Debug.LogError($"{GetType().Name}: {nameof(button)} is null.");

            button.onClick.AddListener(method);
            managedButtons.Add(button);
        }

        AddButtonListener(newGameButtonRootName, StartNewGame);
        AddButtonListener(exitGameButtonRootName, ExitGame);
    }

    protected override void OnSceneUnloaded()
    {
        managedButtons.ForEach(b => b.onClick.RemoveAllListeners());
        managedButtons.Clear();
    }

    /// <summary>
    /// Intended to be fired primarily by a button.
    /// </summary>
    public void StartNewGame()
    {
        GameManager.UpdateGameState(GameState.NewGame);
    }

    /// <summary>
    /// Intended to be fired primarily by a button.
    /// </summary>
    public void ExitGame()
    {
        Quitter.QuitGame();
    }
}
