using System;
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
    private static GameManager instance;

    public static GameState State { get; private set; }

    private static AudioSource backgroundAudioSource;

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
    }

    private void Start()
    {
        UpdateGameState(startingState);
    }

    public static void UpdateGameState(GameState newState)
    {
        OnStateChanged?.Invoke(State = newState);
    }

    public static void PlayLoopedAudio(AudioClip audioClip, float normalizedMaxVolume)
    {
        backgroundAudioSource.clip = audioClip;
        backgroundAudioSource.volume = normalizedMaxVolume * instance.globalMaxVolume;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.Play();
    }

    public static void StopLoopedAudio()
    {
        backgroundAudioSource.Stop();
    }
}

