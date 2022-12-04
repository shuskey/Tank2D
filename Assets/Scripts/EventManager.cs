using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public static event Action PlayerOneJoinedEvent;
    public static event Action PlayerTwoJoinedEvent;
    public static event Action PlayerOneBaseCampDefeatedEvent;
    public static event Action PlayerTwoBaseCampDefeatedEvent;
    public static event Action PlayerOneTankDefeatedEvent;
    public static event Action PlayerTwoTankDefeatedEvent;
    public static event Action PlayerOneTankHitEvent;
    public static event Action PlayerTwoTankHitEvent;
    public static event Action PlayerOneBaseHitEvent;
    public static event Action PlayerTwoBaseHitEvent;
    public static event Action PlayerOneAssetChangedEvent;
    public static event Action PlayerTwoAssetChangedEvent;
    public static event Action CelebratePlayerOneDefeatedEvent;
    public static event Action CelebratePlayerTwoDefeatedEvent;
    public static event Action PlayerOneWallDeployedEvent;
    public static event Action PlayerTwoWallDeployedEvent;
    public static event Action PlayerOneMineDeployedEvent;
    public static event Action PlayerTwoMineDeployedEvent;
    public static event Action PlayerOneUpdateHUDHealthEvent;
    public static event Action PlayerTwoUpdateHUDHealthEvent;

    public static void StartPlayerOneJoinedEvent() => PlayerOneJoinedEvent?.Invoke();
    public static void StartPlayerTwoJoinedEvent() => PlayerTwoJoinedEvent?.Invoke();
    public static void StartPlayerOneWallDeployedEvent() => PlayerOneWallDeployedEvent?.Invoke();
    public static void StartPlayerTwoWallDeployedEvent() => PlayerTwoWallDeployedEvent?.Invoke();
    public static void StartPlayerOneMineDeployedEvent() => PlayerOneMineDeployedEvent?.Invoke();
    public static void StartPlayerTwoMineDeployedEvent() => PlayerTwoMineDeployedEvent?.Invoke();
    public static void StartPlayerOneBaseCampDefeatedEvent() => PlayerOneBaseCampDefeatedEvent?.Invoke();
    public static void StartPlayerTwoBaseCampDefeatedEvent() => PlayerTwoBaseCampDefeatedEvent?.Invoke();
    public static void StartPlayerOneTankDefeatedEvent() => PlayerOneTankDefeatedEvent?.Invoke();
    public static void StartPlayerTwoTankDefeatedEvent() => PlayerTwoTankDefeatedEvent?.Invoke();
    public static void StartPlayerOneTankHitEvent() => PlayerOneTankHitEvent?.Invoke();
    public static void StartPlayerTwoTankHitEvent() => PlayerTwoTankHitEvent?.Invoke();
    public static void StartPlayerOneBaseHitEvent() => PlayerOneBaseHitEvent?.Invoke();
    public static void StartPlayerTwoBaseHitEvent() => PlayerTwoBaseHitEvent?.Invoke();
    public static void StartPlayerOneAssetChangedEvent() => PlayerOneAssetChangedEvent?.Invoke();
    public static void StartPlayerTwoAssetChangedEvent() => PlayerTwoAssetChangedEvent?.Invoke();
    public static void StartCelebratePlayerOneDefeatedEvent() => CelebratePlayerOneDefeatedEvent?.Invoke();
    public static void StartCelebratePlayerTwoDefeatedEvent() => CelebratePlayerTwoDefeatedEvent?.Invoke();
    public static void StartPlayerOneUpdateHUDHealthEvent() => PlayerOneUpdateHUDHealthEvent?.Invoke();
    public static void StartPlayerTwoUpdateHUDHealthEvent() => PlayerTwoUpdateHUDHealthEvent?.Invoke();

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //    PlayerOneDefeatedEvent?.Invoke();
        //if (Input.GetMouseButtonDown(1))
        //    PlayerTwoDefeatedEvent?.Invoke();
    }
}
