using System.Linq;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuManager : MonoBehaviour
{
    /// <summary>
    /// GameStates where pause should not be active. Currently and probably always will only make sense for MainMenu
    /// </summary>
    [SerializeField]
    private GameState[] unPausableStates = new GameState[] { GameState.MainMenu };

    [SerializeField]
    private string pauseResumeRootName = "PauseResume";

    [SerializeField]
    private string pauseRestartRootName = "PauseRestart";

    [SerializeField]
    private string pauseMainMenuRootName = "PauseMainMenu";

    [SerializeField]
    private string pauseExitGameRootName = "PauseExitGame";

    private CanvasGroup canvasGroup;

    private Canvas canvas;

    private Player player => FindObjectOfType<Player>();

    private CinemachineBrain playerCamera => FindObjectOfType<CinemachineBrain>();

    public bool IsPaused => canvasGroup.interactable;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Utility.LogErrorIfNull(canvasGroup, nameof(canvas));

        canvas = canvasGroup.GetComponent<Canvas>();
        Utility.LogErrorIfNull(canvas, nameof(canvas));

        canvasGroup.alpha = 0; // We don't set this in the inspector because then we can't see it in the inspector! And that's annoying.
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        Utility.AddButtonCallback(pauseResumeRootName, () => SetPauseState(areWePausing: false));
        Utility.AddButtonCallback(pauseRestartRootName, () => GameManager.UpdateGameState(GameManager.State));
        Utility.AddButtonCallback(pauseMainMenuRootName, () => GameManager.UpdateGameState(GameState.MainMenu));
        Utility.AddButtonCallback(pauseExitGameRootName, () => Quitter.QuitGame());
    }

    private void Update()
    {
        if (IsPaused && InUnPauseableState())
        {
            SetPauseState(false);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !InUnPauseableState()) // TODO: Fix this input so that gamepad works
        {
            SetPauseState(!canvasGroup.interactable);
        }
    }

    public void SetPauseState(bool areWePausing)
    {
        canvasGroup.alpha = areWePausing ? 1 : 0;
        canvasGroup.blocksRaycasts = areWePausing;
        canvasGroup.interactable = areWePausing;

        if (player != null) player.enabled = !areWePausing;
        if (playerCamera != null) playerCamera.enabled = !areWePausing;

        if (areWePausing && !TimeScaleToggle.IsTimePaused || !areWePausing && TimeScaleToggle.IsTimePaused)
            TimeScaleToggle.Toggle();

        if (areWePausing)
            Utility.DisableAllOf(except: canvas);
        else
            Utility.EnableAllOf(except: canvas);
    }

    private bool InUnPauseableState()
    {
        return unPausableStates.Any(s => s == GameManager.State);
    }
}
