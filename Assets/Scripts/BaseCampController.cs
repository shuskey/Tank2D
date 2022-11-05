using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BaseCampController : MonoBehaviour
{
    [SerializeField] private GameObject[] player1AssetPrefabs;
    [SerializeField] private GameObject[] player2AssetPrefabs;
    [SerializeField] private LayerMask whatStopsMovement;
    [SerializeField] private int wallInventory = 10;
    [SerializeField] private int mineInventory = 10;

    private GameObject[] playerAssetGameObjects = new GameObject[5];    
    private AssetController[] playerAssetControllerScipt = new AssetController[5];    
    private int playerIndexThatOwnsThisBaseCamp = 0;

    private const int worldUnitsPerGrid = 3;

    private int currentAssetBeingControlledIndex = 0;
    private int assetBufferSize = 5;

    private Vector2[] baseCampStartPositions = { new Vector2(-6, 3), new Vector2(55*3, -24) };
    private Vector2[] assetStartPositions = { Vector2.zero, Vector2.up * 3, Vector2.right * 3, Vector2.down * 3, Vector2.left * 3 };
    private float[] assetStartRotationAngles = { 0, 0, -90, 180, 90};

    private Camera assetFollowCamera;

    private void Awake()
    {
        playerIndexThatOwnsThisBaseCamp = GetComponent<PlayerInput>().playerIndex;
        assetFollowCamera = GetComponentInChildren<Camera>();
        var maskNameToUse = (playerIndexThatOwnsThisBaseCamp == 0) ? "OnlyForPlayerOne" : "OnlyForPlayerTwo";

        // Make camera so it only shows their own land mines and not the other players
        assetFollowCamera.cullingMask |= (1 << LayerMask.NameToLayer(maskNameToUse));
        
        int index = 0;
        foreach (var assetPrefab in (playerIndexThatOwnsThisBaseCamp == 0 ? player1AssetPrefabs: player2AssetPrefabs))
        {
            var newInstantiatedGameObject = Instantiate(assetPrefab,
                (Vector3)assetStartPositions[index],
                Quaternion.identity,
                gameObject.transform);
            var baseCampAssetGameObject = new List<GameObject>
                (GameObject.FindGameObjectsWithTag("BaseCampAsset")).
                Find(g => g.transform.IsChildOf(newInstantiatedGameObject.transform));
            baseCampAssetGameObject.transform.rotation = Quaternion.AngleAxis(assetStartRotationAngles[index], Vector3.forward);
            playerAssetGameObjects[index] = newInstantiatedGameObject;
            playerAssetControllerScipt[index] = newInstantiatedGameObject.GetComponentInChildren<AssetController>();
            playerAssetControllerScipt[index].InitializeAssetForBaseCamp(playerIndexThatOwnsThisBaseCamp, wallInventory, mineInventory);
            index++;
        }

        // Set BaseCamp position        
        transform.position = BaseCampPlacement.GetRandomViableBasePosition(whatStopsMovement) * worldUnitsPerGrid;
    }

    private void Start()
    {
        SelectThisAssetIndex(currentAssetBeingControlledIndex);
    }  

    private void SelectNextActiveAsset(int direction)
    {        
        var nextNewAssetIndex = currentAssetBeingControlledIndex;
        var retry = 5;
        do { 
            nextNewAssetIndex = getNextArrayIndex(nextNewAssetIndex, direction); 
        } while (retry-- != 0 && !playerAssetGameObjects[nextNewAssetIndex].activeSelf);
        SelectThisAssetIndex(nextNewAssetIndex);        
    }

    private int getNextArrayIndex(int currentIndex, int direction)
    {
        // mod % will melt your mind because it returns negative numbers when decrementing 
        return ((currentIndex + direction) % assetBufferSize + assetBufferSize) % assetBufferSize;
    }

    private void SelectThisAssetIndex(int newAssetIndex)
    {
        // Unselect previous Asset
        playerAssetControllerScipt[currentAssetBeingControlledIndex].AssetRemoteControlEngaged(false);
        currentAssetBeingControlledIndex = newAssetIndex;
        // Select new Asset
        playerAssetControllerScipt[currentAssetBeingControlledIndex].AssetRemoteControlEngaged(true);
        assetFollowCamera.GetComponentInChildren<CameraFollow>().
            SetAssetToFollow(playerAssetGameObjects[currentAssetBeingControlledIndex].transform.GetChild(0).gameObject);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerAssetControllerScipt[currentAssetBeingControlledIndex].MoveButtonPressed(context.ReadValue<Vector2>());
    }

    public void OnShootButtonPressed(InputAction.CallbackContext context)
    {
        playerAssetControllerScipt[currentAssetBeingControlledIndex].ShootButtonPressed(context.action.triggered);
    }

    public void OnExitKeyPressed()
    {
        Debug.Log("Exit Key Pressed");
        Application.Quit();
    }

    public void OnDropWallPressed(InputAction.CallbackContext context)
    {
        playerAssetControllerScipt[currentAssetBeingControlledIndex].DropWallButtonPressed(context.action.triggered);
    }

    public void OnDropMinePressed(InputAction.CallbackContext context)
    {                  
        playerAssetControllerScipt[currentAssetBeingControlledIndex].DropMineButtonPressed(context.action.triggered);
    }

    public void OnSelectNextAsset(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            SelectNextActiveAsset(1);
    }

    public void OnSelectPreviousAsset(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            SelectNextActiveAsset(-1);
    }
}
