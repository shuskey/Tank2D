using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasAssignMonitor : MonoBehaviour
{
    [SerializeField] private Canvas myCanvas;

    void Awake()
    {
        var playerIndex = GetComponent<PlayerInput>().playerIndex;
       // Debug.Log($"This is playerIndex {playerIndex}");
        if (playerIndex == 1 && myCanvas != null)
        {
            myCanvas.targetDisplay = 1;                        
        }
    }
}
