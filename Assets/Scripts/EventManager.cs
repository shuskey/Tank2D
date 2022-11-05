using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{    
    public static event Action PlayerOneBaseCampDefeatedEvent;
    public static event Action PlayerTwoBaseCampDefeatedEvent;
    public static event Action PlayerOneTankDefeatedEvent;
    public static event Action PlayerTwoTankDefeatedEvent;
    public static event Action CelebratePlayerOneDefeatedEvent;
    public static event Action CelebratePlayerTwoDefeatedEvent;
    public static event Action PlayerOneWallDeployedEvent;
    public static event Action PlayerTwoWallDeployedEvent;
    public static event Action PlayerOneMineDeployedEvent;
    public static event Action PlayerTwoMineDeployedEvent;

    public static void StartPlayerOneWallDeployedEvent() => PlayerOneWallDeployedEvent?.Invoke();
    public static void StartPlayerTwoWallDeployedEvent() => PlayerTwoWallDeployedEvent?.Invoke();
    public static void StartPlayerOneMineDeployedEvent() => PlayerOneMineDeployedEvent?.Invoke();
    public static void StartPlayerTwoMineDeployedEvent() => PlayerTwoMineDeployedEvent?.Invoke();
    public static void StartPlayerOneBaseCampDefeatedEvent() => PlayerOneBaseCampDefeatedEvent?.Invoke();
    public static void StartPlayerTwoBaseCampDefeatedEvent() => PlayerTwoBaseCampDefeatedEvent?.Invoke();
    public static void StartPlayerOneTankDefeatedEvent() => PlayerOneTankDefeatedEvent?.Invoke();
    public static void StartPlayerTwoTankDefeatedEvent() => PlayerTwoTankDefeatedEvent?.Invoke();
    public static void StartCelebratePlayerOneDefeatedEvent() => CelebratePlayerOneDefeatedEvent?.Invoke();
    public static void StartCelebratePlayerTwoDefeatedEvent() => CelebratePlayerTwoDefeatedEvent?.Invoke();

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //    PlayerOneDefeatedEvent?.Invoke();
        //if (Input.GetMouseButtonDown(1))
        //    PlayerTwoDefeatedEvent?.Invoke();
    }
}
