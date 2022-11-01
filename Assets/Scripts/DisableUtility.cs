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
            EventManager.StartPlayerOneDefeatedEvent();
        if (playerNumber == 2)
            EventManager.StartPlayerTwoDefeatedEvent();
    }
}
