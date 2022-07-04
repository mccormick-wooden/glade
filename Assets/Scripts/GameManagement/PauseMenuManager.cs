using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuManager : MonoBehaviour
{
    /// <summary>
    /// GameStates where pause should not be active. Currently and probably always will only make sense for MainMenu
    /// </summary>
    [SerializeField]
    private GameState[] unPausableStates = new GameState[] { GameState.MainMenu };

    [SerializeField]
    private GameState[] pauseBackgroundAudioStates = new GameState[] { GameState.NewGame };

    [SerializeField]
    private string pauseResumeRootName = "PauseResume";

    [SerializeField]
    private string pauseRestartRootName = "PauseRestart";

    [SerializeField]
    private string pauseMainMenuRootName = "PauseMainMenu";

    [SerializeField]
    private string pauseExitGameRootName = "PauseExitGame";

    [SerializeField]
    private string transitionCanvasRootName = "Transitions";
    private Canvas transitionCanvas;

    [SerializeField]
    private string newGameCrawlCanvasRootName = "NewGameCrawl";

    private CharacterPlayerControls controls;

    private CanvasGroup canvasGroup;

    private Canvas pauseCanvas;

    private Player player => FindObjectOfType<Player>();

    private CinemachineBrain playerCamera => FindObjectOfType<CinemachineBrain>();

    public bool IsPaused => canvasGroup.interactable;

    private void Awake()
    {
        controls = new CharacterPlayerControls();

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
        Utility.AddButtonCallback(pauseRestartRootName, () => GameManager.instance.UpdateGameState(GameManager.instance.State));
        Utility.AddButtonCallback(pauseMainMenuRootName, () => GameManager.instance.UpdateGameState(GameState.MainMenu));
        Utility.AddButtonCallback(pauseExitGameRootName, () => Quitter.QuitGame());
    }

    private void Start()
    {
        controls.PauseGame.PauseGameAction.performed += _ => TogglePause();
        GameManager.OnStateChanged += GameManagerOnStateChanged;
    }

    private void OnDestroy()
    {
        controls.PauseGame.PauseGameAction.performed -= _ => TogglePause();
        GameManager.OnStateChanged -= GameManagerOnStateChanged;
    }

    private void TogglePause()
    {
        if (!IsPaused && !InUnPauseableState())
            SetPauseState(true);
        else if (IsPaused)
            SetPauseState(false);
    }

    private void OnEnable()
    {
        controls.PauseGame.Enable();
    }

    private void OnDisable()
    {
        controls.PauseGame.Disable();
    }

    public void SetPauseState(bool areWePausing)
    {
        if (IsPauseBackgroundAudioState())
            GameManager.instance.ToggleLoopedAudio(areWePausing);

        canvasGroup.alpha = areWePausing ? 1 : 0;
        canvasGroup.blocksRaycasts = areWePausing;
        canvasGroup.interactable = areWePausing;

        if (player != null) player.enabled = !areWePausing;
        if (playerCamera != null) playerCamera.enabled = !areWePausing;

        if (areWePausing && !TimeScaleToggle.IsTimePaused || !areWePausing && TimeScaleToggle.IsTimePaused)
            TimeScaleToggle.Toggle();

        if (areWePausing)
        {
            Utility.DisableAllOf(except: new Canvas[] {
                pauseCanvas,
                transitionCanvas,
                GameObject.Find(newGameCrawlCanvasRootName)?.GetComponentInChildren<Canvas>()});
        }
        else
        {
            Utility.EnableAllOf(except: pauseCanvas);
        }

        if (areWePausing)
        {
            GameObject.Find(pauseResumeRootName).GetComponentInChildren<Button>().Select();
        }
    }

    private bool InUnPauseableState()
    {
        return unPausableStates.Any(s => s == GameManager.instance.State);
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
