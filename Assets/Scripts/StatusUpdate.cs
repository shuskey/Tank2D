using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusUpdate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Canvas myCanvas;

    private void Start()
    {
        EventManager.PlayerOneDefeatedEvent += ShowPlayerOneDefeated;
        EventManager.PlayerTwoDefeatedEvent += ShowPlayerTwoDefeated;
    }

    private void OnDisable()
    {
        EventManager.PlayerOneDefeatedEvent -= ShowPlayerOneDefeated;
        EventManager.PlayerOneDefeatedEvent -= ShowPlayerTwoDefeated;
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
        if (myCanvas.targetDisplay == playerIndexThatWasDefeated)
            statusText.text = "Defeated!";
        else
            statusText.text = "Victory!";
    }
}
