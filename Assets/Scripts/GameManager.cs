using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static GameState State;
    public static event Action<GameState> OnGameStateChanged;

    // Start is called before the first frame update
    void Start()
    {
        UpdateGameState(GameState.Welcome);
    }

    public static void OnPlay()
    {
        UpdateGameState(GameState.Play);
    }

    public static void OnWelcome()
    {
        UpdateGameState(GameState.Welcome);
    }

    public static void OnSettings()
    {
        UpdateGameState(GameState.Settings);
    }

    public static void OnInformation()
    {
        UpdateGameState(GameState.Information);
    }

    public static void OnQuitGame()
    {
        UpdateGameState(GameState.Quit);
    }

    public static void OnBackArrow()
    {
        if (State == GameState.Play)
        {
            UpdateGameState(GameState.Welcome);
        }
        else
        {
            if (State == GameState.Information ||
                State == GameState.Settings)
                UpdateGameState(GameState.Welcome);
        }
    }

    public static GameState GetCurrentGameState() => State;

    private static void UpdateGameState(GameState newState)
    {

        switch (newState)
        {
            case GameState.Welcome:
                // MenuManager is subscribed to this
                break;
            case GameState.Play:
                // PoolTableManager is subscribed to this
                break;
            case GameState.Victory:
                if (State == GameState.Failure || State == GameState.Victory)
                    return;
                // Window_Confetti is subscribed to this
                break;
            case GameState.Failure:
                if (State == GameState.Failure || State == GameState.Victory)
                    return;
                break;
            case GameState.Quit:
                Debug.Log("Quit Game Requested");
#if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
            default:
                break;
        }

        State = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    public enum GameState
    {
        Welcome,
        Settings,
        Information,        
        Play,
        Victory,        
        Failure, 
        Quit
    }
}

