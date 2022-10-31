using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{    
    public static event Action PlayerOneDefeatedEvent;
    public static event Action PlayerTwoDefeatedEvent;

    public static void StartPlayerOneDefeatedEvent()
    {
        PlayerOneDefeatedEvent?.Invoke();
    }

    public static void StartPlayerTwoDefeatedEvent()
    {
        PlayerTwoDefeatedEvent?.Invoke();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            PlayerOneDefeatedEvent?.Invoke();
        if (Input.GetMouseButtonDown(1))
            PlayerTwoDefeatedEvent?.Invoke();
    }
}