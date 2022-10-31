using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPanelUpdates : MonoBehaviour
{

    [SerializeField] private TMP_Text bottomDisplayText;
    // Start is called before the first frame update
    void Start()
    {
        var playerIndex = GetComponentInParent<Canvas>().worldCamera.targetDisplay;
        bottomDisplayText.text = playerIndex == 0 ?
            "Welcome Team Purple Perfect !" :
            "Welcome Green Team of Awesomeness !";

    }
    private void Awake()
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
        if (GetComponentInParent<Canvas>().worldCamera.targetDisplay == playerIndexThatWasDefeated)
            bottomDisplayText.text = "Defeated !";
        else
            bottomDisplayText.text = "Victory !";
    }
}
