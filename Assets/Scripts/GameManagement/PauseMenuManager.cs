using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuManager : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private Player player => FindObjectOfType<Player>();
    private CinemachineBrain playerCamera => FindObjectOfType<CinemachineBrain>();

    private readonly string[] unPausableScenes = new string[] { "MainMenu" };

    public bool IsPaused => canvasGroup.interactable;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup not found");
        }
        else
        {
            canvasGroup.alpha = 0; // We don't set this in the inspector because then we can't see it in the inspector! And that's annoying.
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (IsPaused && InUnPauseableScene())
        {
            SetPauseState(false);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !InUnPauseableScene()) // TODO: Fix this input so that gamepad works
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
    }

    private bool InUnPauseableScene()
    {
        return unPausableScenes.Any(s => SceneManager.GetSceneByName(s).isLoaded); // todo: replace this with game mananager state check maybe?
    }
}
