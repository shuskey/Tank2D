using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject welcomePanel;    
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject informationPanel;        
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
        welcomePanel.SetActive(state == GameState.Welcome);        
        settingsPanel?.SetActive(state == GameState.Settings);
        informationPanel?.SetActive(state == GameState.Information);        
        gameStateText.enabled = true; // Helps with debugging (state != GameState.Welcome);
        gameStateText.text = state.ToString();
    }
}

