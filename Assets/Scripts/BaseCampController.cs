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
    [SerializeField] private GameObject[] assetGameObjects;

    private AssetController[] assetControllerScipt = new AssetController[5];

    private int currentAssetBeingControlledIndex = 0;
    private int numberOfAssetsInBaseCamp = 4;

    private Vector2[] startPositions = { new Vector2(-6, 3), new Vector2(12, 9) };

    private Camera assetFollowCamera;

    private void Awake()
    {
        assetFollowCamera = GetComponentInChildren<Camera>();

        var playerIndex = GetComponent<PlayerInput>().playerIndex;
        transform.position = startPositions[playerIndex];

        int index = 0;
        foreach (var assetGameObject in assetGameObjects)
        {
            assetControllerScipt[index] = assetGameObject.GetComponentInChildren<AssetController>();
            index++;
        }
        assetControllerScipt[currentAssetBeingControlledIndex].AssetRemoteControlEngaged(true);
    }

    public void OnSelectNextAsset(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            assetControllerScipt[currentAssetBeingControlledIndex].AssetRemoteControlEngaged(false);

            var potentialNewIndex = currentAssetBeingControlledIndex;
            var retry = 5;
            do
            {
                potentialNewIndex = potentialNewIndex == numberOfAssetsInBaseCamp ? 0 : potentialNewIndex + 1;
            } while (retry-- != 0 && !assetGameObjects[potentialNewIndex].activeSelf);
            currentAssetBeingControlledIndex = potentialNewIndex;

            assetControllerScipt[currentAssetBeingControlledIndex].AssetRemoteControlEngaged(true);
            assetFollowCamera.GetComponentInChildren<CameraFollow>().
                SetAssetToFollow(assetGameObjects[currentAssetBeingControlledIndex].transform.GetChild(0).gameObject);
        }
    }

    public void OnSelectPreviousAsset(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            assetControllerScipt[currentAssetBeingControlledIndex].AssetRemoteControlEngaged(false);

            var potentialNewIndex = currentAssetBeingControlledIndex;
            var retry = 5;
            do
            {
                potentialNewIndex = potentialNewIndex == 0 ? numberOfAssetsInBaseCamp : potentialNewIndex - 1;
            } while (retry-- != 0 && !assetGameObjects[potentialNewIndex].activeSelf);
            currentAssetBeingControlledIndex = potentialNewIndex;

            assetControllerScipt[currentAssetBeingControlledIndex].AssetRemoteControlEngaged(true);
            assetFollowCamera.GetComponentInChildren<CameraFollow>().
                SetAssetToFollow(assetGameObjects[currentAssetBeingControlledIndex].transform.GetChild(0).gameObject);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        assetControllerScipt[currentAssetBeingControlledIndex].MoveButtonPressed(context.ReadValue<Vector2>());
    }

    public void OnShootButtonPressed(InputAction.CallbackContext context)
    {
        assetControllerScipt[currentAssetBeingControlledIndex].ShootButtonPressed(context.action.triggered);
    }
}
