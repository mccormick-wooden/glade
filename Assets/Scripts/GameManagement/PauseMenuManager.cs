using System.Linq;
using Cinemachine;
using PowerUps;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuManager : MonoBehaviour
{
    [SerializeField]
    private string pauseResumeRootName = "PauseResume";

    [SerializeField]
    private string pauseRestartRootName = "PauseRestart";

    [SerializeField]
    private string pauseMainMenuRootName = "PauseMainMenu";

    [SerializeField]
    private string pauseExitGameRootName = "PauseExitGame";

    [SerializeField] private string transitionCanvasRootName = "Transitions";
    private Canvas transitionCanvas;

    [SerializeField]
    private string newGameCrawlCanvasRootName = "NewGameCrawl";

    [SerializeField]
    private string treeSpiritDialogueCanvasRootName = "TreeSpiritDialogueCanvas"; // possibly have some way of dynamically finding all dialogue canvases

    [SerializeField]
    private string sceneSkipperCanvasRootName = "SceneSkipper"; // ugh this was so stupid

    [SerializeField]
    private string noSelectButtonName = "PauseNoSelect";

    private GameState[] unPauseableStates;

    private GameState[] pauseBackgroundAudioStates;

    private CharacterPlayerControls controls;

    private CanvasGroup canvasGroup;

    private Canvas pauseCanvas;

    private Player player => FindObjectOfType<Player>();

    private CinemachineBrain playerCamera => FindObjectOfType<CinemachineBrain>();

    public bool IsPaused => canvasGroup.interactable;

    private bool playerPrePauseState = true;

    private void Awake()
    {
        unPauseableStates = new GameState[] { GameState.Invalid, GameState.MainMenu };
        pauseBackgroundAudioStates = new GameState[] { GameState.NewGame };

        controls = new CharacterPlayerControls();
        controls.Gameplay.Disable(); // We don't need the Gameplay mapping in this manager

        canvasGroup = GetComponent<CanvasGroup>();
        Utility.LogErrorIfNull(canvasGroup, nameof(pauseCanvas));

        pauseCanvas = canvasGroup.GetComponent<Canvas>();
        Utility.LogErrorIfNull(pauseCanvas, nameof(pauseCanvas));

        transitionCanvas = GameObject.Find(transitionCanvasRootName)?.GetComponentInChildren<Canvas>();
        Utility.LogErrorIfNull(transitionCanvas, nameof(transitionCanvas));

        canvasGroup.alpha = 0; // We don't set this in the inspector because then we can't see it in the inspector! And that's annoying.
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        Utility.AddButtonCallback(pauseResumeRootName, () => SetPauseState(areWePausing: false));
        Utility.AddButtonCallback(pauseRestartRootName,
            () => GameManager.instance.UpdateGameState(GameManager.instance.State));
        Utility.AddButtonCallback(pauseMainMenuRootName,
            () => GameManager.instance.UpdateGameState(GameState.MainMenu));
        Utility.AddButtonCallback(pauseExitGameRootName, () => Quitter.QuitGame());
    }

    private void ShowCursorIfPaused()
    {
        if (IsPaused) Utility.ShowCursor();
    }

    private void Start()
    {
        controls.PauseGame.PauseGameAction.performed += _ => TogglePause();
        controls.PauseGame.MouseMove.performed += _ => ShowCursorIfPaused();
        GameManager.OnStateChanged += GameManagerOnStateChanged;
    }

    private void OnDestroy()
    {
        controls.PauseGame.PauseGameAction.performed -= _ => TogglePause();
        controls.PauseGame.MouseMove.performed -= _ => ShowCursorIfPaused();
        GameManager.OnStateChanged -= GameManagerOnStateChanged;
    }

    private void TogglePause()
    {
        if (GameManager.instance.IsMidTransition)
        {
            return;
        }

        if (!IsPaused && !InUnPauseableState())
            SetPauseState(true);
        else if (IsPaused)
            SetPauseState(false);

        // Either way, we want to hide the cursor on returning to the game or
        // on first entering pause menu
        Utility.HideCursor();
    }

    private void OnEnable()
    {
        controls.PauseGame.Enable();
    }

    private void OnDisable()
    {
        controls.PauseGame.Disable();
    }

    private void SelectButton(string buttonName)
    {
        GameObject.Find(buttonName)?.GetComponentInChildren<Button>()?.Select();
    }

    private void Update()
    {
        // If we mouse click on canvas, it deselects all buttons and makes
        // keyboard/gamepad navigation broken. Select the invisible button
        // instead.
        if (EventSystem.current.currentSelectedGameObject == null)
            SelectButton(noSelectButtonName);
    }

    public void SetPauseState(bool areWePausing)
    {
        if (IsPauseBackgroundAudioState())
        {
            GameManager.instance.ToggleLoopedAudio(areWePausing);
        }

        canvasGroup.alpha = areWePausing ? 1 : 0;
        canvasGroup.blocksRaycasts = areWePausing;
        canvasGroup.interactable = areWePausing;

        if (player != null)
        {
            if (areWePausing)
            {
                playerPrePauseState = player.ControlsEnabled;
                player.UpdateControlState(false);
            }
            else
            {
                player.UpdateControlState(playerPrePauseState);
            }
        }


        if (areWePausing && !TimeScaleToggle.IsTimePaused || !areWePausing && TimeScaleToggle.IsTimePaused)
            TimeScaleToggle.Toggle();

        if (areWePausing)
        {   // TODO: This disble/enable stuff was a bad idea. We should replace code that ensures the pause menu is at the closest point of the foreground on top of every canvas
            Utility.DisableAllOf(except: new Canvas[]
            {
                pauseCanvas,
                transitionCanvas,
                GameObject.Find(newGameCrawlCanvasRootName)?.GetComponentInChildren<Canvas>(),
                GameObject.Find(treeSpiritDialogueCanvasRootName)?.GetComponentInChildren<Canvas>(),
                GameObject.Find(sceneSkipperCanvasRootName)?.GetComponentInChildren<Canvas>(),
                GameObject.Find("EndGameMenu")?.GetComponentInChildren<Canvas>()
            });
        }
        else
        {
            Utility.EnableAllOf(except: new Canvas[]
            {
                pauseCanvas,
                GameObject.Find(treeSpiritDialogueCanvasRootName)?.GetComponentInChildren<Canvas>(),
                GameObject.Find(sceneSkipperCanvasRootName)?.GetComponentInChildren<Canvas>(),
                GameObject.Find("EndGameMenu")?.GetComponentInChildren<Canvas>()
            });
        }

        if (areWePausing)
            SelectButton(pauseResumeRootName);
    }

    private bool InUnPauseableState()
    {
        // Ideally the interaction with the power up menu is a state unto itself
        // For now it's (active!) presence signals we are interacting with that menu
        return unPauseableStates.Any(s => s == GameManager.instance.State) 
            || FindObjectOfType<PowerUpMenu>() 
            || FindObjectOfType<EndGameMenu>().enabled;
    }

    private bool IsPauseBackgroundAudioState()
    {
        return pauseBackgroundAudioStates.Any(s => s == GameManager.instance.State);
    }

    private void GameManagerOnStateChanged(GameState obj)
    {
        // I'm like 99.99% sure we always want to unpause on a state change.
        SetPauseState(areWePausing: false);
    }
}
