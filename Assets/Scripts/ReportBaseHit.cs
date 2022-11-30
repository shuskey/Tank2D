using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportBaseHit : MonoBehaviour
{
    public void ThisBaseIsHit(int playerNumber)
    {
        if (playerNumber == 1)
            EventManager.StartPlayerOneBaseHitEvent();
        if (playerNumber == 2)
            EventManager.StartPlayerTwoBaseHitEvent();
    }
}
