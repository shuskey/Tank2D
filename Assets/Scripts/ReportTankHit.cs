using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportTankHit : MonoBehaviour
{
    public void ThisTankIsHit(int playerNumber)
    {
        if (playerNumber == 1)
            EventManager.StartPlayerOneTankHitEvent();
        if (playerNumber == 2)
            EventManager.StartPlayerTwoTankHitEvent();
    }
}
