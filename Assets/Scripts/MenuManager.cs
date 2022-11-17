using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject scoreBoardPanel;
    [SerializeField] private TMP_Text gameStateText;

    private void Awake()
    {       
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void Start()
    {
        GameManagerOnGameStateChanged(GameManager.GetCurrentGameState());
    }

    private void GameManagerOnGameStateChanged(GameState state)
    {
        welcomePanel.SetActive(state == GameState.WaitingToJoin);        
        scoreBoardPanel?.SetActive(state == GameState.ScoreBoard);             
        gameStateText.enabled = true; // Helps with debugging (state != GameState.Welcome);
        gameStateText.text = state.ToString();
    }
}

