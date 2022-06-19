using System;
using UnityEngine;

public enum GameState
{
    Invalid = 0,
    MainMenu = 1,
    InGame = 2,
    GameWon = 3,
    GameLost = 4,
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public GameState State { get; private set; }

    public static event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateGameState(GameState.MainMenu);
    }

    public void UpdateGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                break;
            case GameState.InGame:
                break;
            case GameState.GameWon:
                break;
            case GameState.GameLost:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, "This state is unimplemented");
        }

        State = newState;

        OnStateChanged?.Invoke(State);
    }
}

