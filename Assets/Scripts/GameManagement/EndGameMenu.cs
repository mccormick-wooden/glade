using PlayerBehaviors;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class EndGameMenu : MonoBehaviour
{
    private Canvas _canvas;

    public GameObject firstOptionButton;
    public GameObject secondOptionButton;
    public GameObject thirdOptionButton;
    public GameObject noSelectButton;

    private Player Player => FindObjectOfType<Player>();

    private CharacterPlayerControls controls;

    private TextMeshProUGUI textBox;

    public void SetActive(bool active)
    {
        _canvas.enabled = active;
        enabled = active;
    }

    public void SetText(string text)
    {
        textBox.text = text;
    }

    private void Awake()
    {
        textBox = GetComponentInChildren<TextMeshProUGUI>();
        _canvas = GetComponentInChildren<Canvas>();
        _canvas.enabled = false;
        enabled = false;
    }

    private void Update()
    {
        // If we mouse click on canvas, it deselects all buttons and makes
        // keyboard/gamepad navigation broken. Select the invisible button
        // instead.
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(noSelectButton);
    }

    private void OnEnable()
    {
        SetPauseState(true);

        // UI Specific
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstOptionButton);

        // Heavy-handed way to keep cursor hidden until mouse moves
        controls = new CharacterPlayerControls();
        controls.Gameplay.Disable();
        controls.PauseGame.Enable();
        controls.PauseGame.MouseMove.performed += _ => {
            if (isActiveAndEnabled)
                Utility.ShowCursor();
        };
        Utility.HideCursor();

        SetupButtonCallbacks();
    }

    private void OnDisable()
    {
        TeardownButtonCallbacks();
        SetPauseState(false);
        Utility.HideCursor();
        controls?.Dispose();
        SetActive(false);
    }

    private void SetPauseState(bool areWePausing)
    {
        if (Player != null) Player.enabled = !areWePausing;

        if (areWePausing && !TimeScaleToggle.IsTimePaused || !areWePausing && TimeScaleToggle.IsTimePaused)
            TimeScaleToggle.Toggle();

        if (areWePausing)
        {
            Utility.DisableAllOf(except: PlayerStats.Instance);
            _canvas.enabled = true;
        }
        else
            Utility.EnableAllOf(except: new Canvas[] { _canvas,
                GameObject.Find("TreeSpiritDialogueCanvas")?.GetComponentInChildren<Canvas>() });
    }

    private void SetupButtonCallbacks()
    {
        Utility.AddButtonCallback("EndGameRestart", () => GameManager.instance.UpdateGameState(GameManager.instance.State));
        Utility.AddButtonCallback("EndGameMainMenu", () => GameManager.instance.UpdateGameState(GameState.MainMenu));
        Utility.AddButtonCallback("EndGameExitGame", () => Quitter.QuitGame());
    }

    private void TeardownButtonCallbacks()
    {
        Utility.ClearButtonAllCallbacks("EndGameRestart");
        Utility.ClearButtonAllCallbacks("EndGameMainMenu");
        Utility.ClearButtonAllCallbacks("EndGameExitGame");
    }
}
