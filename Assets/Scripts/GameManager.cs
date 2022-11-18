using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static GameState State;
    public static event Action<GameState> OnGameStateChanged;
    public static int playerOneScore = 0;
    public static int playerTwoScore = 0;


    // Start is called before the first frame update
    void Start()
    {
        playerOneScore = playerTwoScore = 0;
        UpdateGameState(GameState.WaitingToJoin);
    }

    public static void OnPlay()
    {
        UpdateGameState(GameState.Play);
    }

    public static void OnPlayerOneVictory()
    {
        playerOneScore++;
        UpdateGameState(GameState.ScoreBoard);
    }

    public static void OnPlayerTwoVictory()
    {
        playerTwoScore++;
        UpdateGameState(GameState.ScoreBoard);
    }

    public static void OnWelcome()
    {
        UpdateGameState(GameState.WaitingToJoin);
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
            UpdateGameState(GameState.WaitingToJoin);
        } else
        {
            Debug.Log("** Application Quit being called **");
            Application.Quit();
        }
    }

    public static void OnScoreBoard()
    {
        UpdateGameState(GameState.ScoreBoard);
    }

    public static void OnOkContinue()
    {
        if (State == GameState.WaitingToJoin)
        {
            UpdateGameState(GameState.Play);
        }
        if (State == GameState.ScoreBoard)
        {
            UpdateGameState(GameState.Play);            
        }
    }

    public static GameState GetCurrentGameState() => State;

    private static void UpdateGameState(GameState newState)
    {

        switch (newState)
        {
            case GameState.WaitingToJoin:                           
                break;
            case GameState.Play:
                break;
            case GameState.PlayerOneVictory:                                
                break;
            case GameState.PlayerTwoVictory:                                    
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
        WaitingToJoin,
        PlayPaused,
        ScoreBoard,
        Settings,
        Information,        
        Play,
        PlayerOneVictory,        
        PlayerTwoVictory, 
        Quit
    }
}

