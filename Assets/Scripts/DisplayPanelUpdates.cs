using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPanelUpdates : MonoBehaviour
{

    [SerializeField] private TMP_Text bottomDisplayText;
    private int playerIndex;
    private int playerOneTankCount = 4;
    private int playerTwoTankCount = 4;

    // Start is called before the first frame update
    void Start()
    {
        playerIndex = GetComponentInParent<Canvas>().worldCamera.targetDisplay;
        bottomDisplayText.text = playerIndex == 0 ?
            "Welcome Team Purple Perfect !" :
            "Welcome Green Team of Awesomeness !";

    }
    private void Awake()
    {
        EventManager.CelebratePlayerOneDefeatedEvent += ShowPlayerOneDefeated;
        EventManager.CelebratePlayerTwoDefeatedEvent += ShowPlayerTwoDefeated;

        EventManager.PlayerOneTankDefeatedEvent += PlayerOneTankDefeated;
        EventManager.PlayerTwoTankDefeatedEvent += PlayerTwoTankDefeated;
    }
    
    private void OnDisable()
    {
        EventManager.CelebratePlayerOneDefeatedEvent -= ShowPlayerOneDefeated;
        EventManager.CelebratePlayerTwoDefeatedEvent -= ShowPlayerTwoDefeated;

        EventManager.PlayerOneTankDefeatedEvent -= PlayerOneTankDefeated;
        EventManager.PlayerTwoTankDefeatedEvent -= PlayerTwoTankDefeated;
    }

    private void PlayerOneTankDefeated()
    {
        playerOneTankCount--;
        UpdateDisplayStatus();
    }

    private void PlayerTwoTankDefeated()
    {
        playerTwoTankCount--;
        UpdateDisplayStatus();
    }

    private void UpdateDisplayStatus()
    {
        bottomDisplayText.text = playerIndex == 0 ?
            $"Purple Perfect, Mine:{playerOneTankCount}, Enemy:{playerTwoTankCount}" :
            $"Green Awesomeness, Mine:{playerTwoTankCount}, Enemy:{playerOneTankCount}";

        if (playerOneTankCount == 0)
            EventManager.StartCelebratePlayerOneDefeatedEvent();
        if (playerTwoTankCount == 0)
            EventManager.StartCelebratePlayerTwoDefeatedEvent();
    }

    private void ShowPlayerOneDefeated()
    {
        ShowDefeatedVictoryStatus(0);
    }

    private void ShowPlayerTwoDefeated()
    {
        ShowDefeatedVictoryStatus(1);
    }

    private void ShowDefeatedVictoryStatus(int playerIndexThatWasDefeated)
    {
        if (playerIndex == playerIndexThatWasDefeated)
            bottomDisplayText.text = "Defeated !";
        else
            bottomDisplayText.text = "Victory !";
    }
}
