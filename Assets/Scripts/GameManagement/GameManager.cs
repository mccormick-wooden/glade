using System;
using UnityEngine;

public enum GameState
{
    Invalid = 0,
    MainMenu = 10,
    NewGame = 20,
    Level1 = 30,
    GameWon = 40,
    GameLost = 550,
}

public class GameManager : MonoBehaviour // TODO: create other managers, attach them to object, and inject self? 
{
    private static GameManager instance;

    public static GameState State { get; private set; }

    private static AudioSource backgroundAudioSource;

    [SerializeField]
    private GameState startingState = GameState.MainMenu;

    [Header("Player Controlled Game Settings")]
    [SerializeField]
    private static float globalMaxVolume = 1;

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
        // TODO: validations to ensure every scene and state is managed by only one object? 
    }

    private void Start()
    {
        UpdateGameState(startingState);
    }

    public static void UpdateGameState(GameState newState)
    {
        switch (newState) // TODO: What do I even do here? just enforce valid state transitions? 
        {
            case GameState.MainMenu:
                break;
            case GameState.NewGame:
                break;
            case GameState.Level1:
                break;
            case GameState.GameWon:
                break;
            case GameState.GameLost:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, "This state is unimplemented");
        }

        OnStateChanged?.Invoke(State = newState);
    }

    public static void PlayLoopedAudio(AudioClip audioClip, float normalizedMaxVolume)
    {
        backgroundAudioSource.clip = audioClip;
        backgroundAudioSource.volume = normalizedMaxVolume * globalMaxVolume;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.Play();
    }

    public static void StopLoopedAudio()
    {
        backgroundAudioSource.Stop();
    }
}

