using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportTankDestroy : MonoBehaviour
{
public void ThisTankIsDestroyed(int playerNumber)
    {
        if (playerNumber == 1)
            EventManager.StartPlayerOneTankDefeatedEvent();
        if (playerNumber == 2)
            EventManager.StartPlayerTwoTankDefeatedEvent();
    }
}
