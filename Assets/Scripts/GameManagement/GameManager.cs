using System;
using System.Collections;
using UnityEngine;

public enum GameState
{
    Invalid = 0,
    MainMenu = 10,
    NewGame = 20,
    Training = 21,
    Level1 = 30,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState State { get; private set; }

    public bool IsMidTransition { get; private set; }

    [Header("Cursor")]
    public Texture2D cursorTexture;
    public Vector2 cursorTextureTargetPoint;

    private AudioSource backgroundAudioSource;

    private Animator transitionCanvasAnimationController;

    [SerializeField]
    private GameState startingState = GameState.MainMenu;

    [Header("Player Controlled Game Settings")]
    [SerializeField]
    [Range(0f, 1f)]
    private float globalMaxVolume = 1;

    public static event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        transitionCanvasAnimationController = GameObject.Find("Transitions")?.GetComponentInChildren<Animator>();
        Utility.LogErrorIfNull(transitionCanvasAnimationController, nameof(transitionCanvasAnimationController), "Missing transition animator!");
    }

    private void Start()
    {
        UpdateGameState(startingState);
        Cursor.SetCursor(cursorTexture, cursorTextureTargetPoint, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void UpdateGameState(GameState newState, bool withTransition = true)
    {
        if (withTransition)
            instance.InvokeTransition(midTransitionAction: () => OnStateChanged?.Invoke(State = newState));
        else
            OnStateChanged?.Invoke(State = newState);
    }

    public void PlayLoopedAudio(AudioClip audioClip, float normalizedMaxVolume)
    {
        backgroundAudioSource = GetFreeAudioSource();
        backgroundAudioSource.clip = audioClip;
        backgroundAudioSource.volume = normalizedMaxVolume * instance.globalMaxVolume;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.Play();
    }

    public void StopLoopedAudio()
    {
        backgroundAudioSource.Stop();
        FreeAudioSource(backgroundAudioSource);
    }

    public void ToggleLoopedAudio(bool areWePausing)
    {
        if (areWePausing)
            backgroundAudioSource.Pause();
        else
            backgroundAudioSource.Play();
    }

    public void InvokeTransition(Action midTransitionAction = null)
    {
        StartCoroutine(Transition(midTransitionAction));
    }

    private IEnumerator Transition(Action midTransitionAction)
    {
        IsMidTransition = true;

        transitionCanvasAnimationController.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1);

        if (midTransitionAction != null)
            midTransitionAction();

        transitionCanvasAnimationController.SetTrigger("FadeIn");
        yield return new WaitForSecondsRealtime(1);

        IsMidTransition = false;
    }

    public AudioSource GetFreeAudioSource()
    {
        var audioSources = GetComponents<AudioSource>();

        foreach (var src in audioSources)
        {
            if (src.clip == null)
                return src;
        }

        Debug.LogError("No free audio source!");
        return null;
    }

    public void FreeAudioSource(AudioSource src)
    {
        src.clip = null;
    }
}

