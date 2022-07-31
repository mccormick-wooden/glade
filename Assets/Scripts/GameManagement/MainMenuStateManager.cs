using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuStateManager : BaseStateManager
{
    [Header("Scene Settings")]
    [SerializeField]
    private string mainNewGameRootName = "MainNewGame";

    [SerializeField]
    private string mainExitRootName = "MainExitGame";

    [SerializeField]
    private string noSelectButtonName = "NoSelect";

    private CharacterPlayerControls controls;

    protected override void OnSceneLoaded()
    {
        Utility.AddButtonCallback(mainNewGameRootName, () => UpdateNextGameState());
        Utility.AddButtonCallback(mainExitRootName, () => Quitter.QuitGame());
        SelectButton(mainNewGameRootName);

        // Heavy handed way to hide cursor until/unless mouse is moved.
        if (hideCursorOnSceneStart)
        {
            controls = new CharacterPlayerControls();
            controls.Gameplay.Disable();
            controls.PauseGame.Enable();
            controls.PauseGame.MouseMove.performed += _ => Utility.ShowCursor();
            Utility.HideCursor();
        }
    }

    private void Update()
    {
        // If we mouse click on canvas, it deselects all buttons and makes
        // keyboard/gamepad navigation broken. Select the invisible button
        // instead.
        if (EventSystem.current.currentSelectedGameObject == null)
            SelectButton(noSelectButtonName);
    }

    private void SelectButton(string buttonName)
    {
        GameObject.Find(buttonName)?.GetComponentInChildren<Button>()?.Select();
    }

    protected override void OnSceneUnloaded()
    {
        controls?.Dispose();
    }

    protected override void UpdateNextGameState()
    {
        GameManager.instance.UpdateGameState(GameState.NewGame);
    }
}
