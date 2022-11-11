using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AssignMonitor : MonoBehaviour
{
    void Awake()
    {
        var playerIndex = GetComponent<PlayerInput>().playerIndex;
       // Debug.Log($"This is playerIndex {playerIndex}");
        if (playerIndex == 1)
        {
            // Follow camera and map camera that is in BaseAsset
            var allCameras = GetComponentsInChildren<Camera>();
            foreach (var cam in allCameras)
            {                
                cam.targetDisplay = 1;
            }

            var allCanvas = GetComponentsInChildren<Canvas>();
            foreach (var canvas in allCanvas)
            {
                canvas.targetDisplay = 1;
            }

        }
        if (playerIndex == 0)
            EventManager.StartPlayerOneJoinedEvent();
        if (playerIndex == 1)
            EventManager.StartPlayerTwoJoinedEvent();                    
    }
}
