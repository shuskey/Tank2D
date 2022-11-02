using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableUtility : MonoBehaviour
{
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void InvokePlayerDefeatedEvent(int playerNumber)
    {
        if (playerNumber == 1)
        {
            EventManager.StartPlayerOneBaseCampDefeatedEvent();
            EventManager.StartCelebratePlayerOneDefeatedEvent();
        }
        if (playerNumber == 2)
        {
            EventManager.StartPlayerTwoBaseCampDefeatedEvent();
            EventManager.StartCelebratePlayerTwoDefeatedEvent();
        }
    }
}
