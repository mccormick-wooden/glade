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

        backgroundAudioSource = GetComponent<AudioSource>();
        Utility.LogErrorIfNull(backgroundAudioSource, nameof(backgroundAudioSource), "No background audio source component on game manager!");

        transitionCanvasAnimationController = GameObject.Find("Transitions")?.GetComponentInChildren<Animator>();
        Utility.LogErrorIfNull(transitionCanvasAnimationController, nameof(transitionCanvasAnimationController), "Missing transition animator!");
    }

    private void Start()
    {
        UpdateGameState(startingState, withTransition: false);
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
        backgroundAudioSource.clip = audioClip;
        backgroundAudioSource.volume = normalizedMaxVolume * instance.globalMaxVolume;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.Play();
    }

    public void StopLoopedAudio()
    {
        backgroundAudioSource.Stop();
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
        transitionCanvasAnimationController.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1);

        if (midTransitionAction != null)
            midTransitionAction();

        transitionCanvasAnimationController.SetTrigger("FadeIn");
        yield return new WaitForSecondsRealtime(1); 
    }
}

