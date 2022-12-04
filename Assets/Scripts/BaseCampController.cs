using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static GameManager;

public class BaseCampController : MonoBehaviour
{
    [SerializeField] private GameObject[] player1AssetPrefabs;
    [SerializeField] private GameObject[] player2AssetPrefabs;
    [SerializeField] private GameObject displayPanel;
    [SerializeField] private int wallInventory = 30;
    [SerializeField] private int mineInventory = 15;
    [SerializeField] private Tile purpleOilDrip;
    [SerializeField] private Tile greenOilDrip;
    [SerializeField] private Tile neutralOilDrip;

    private GameObject[] playerAssetGameObjects = new GameObject[5];    
    private AssetController[] playerAssetControllerScipt = new AssetController[5];    
    public int playerIndexThatOwnsThisBaseCamp = 0;

    private int currentAssetBeingControlledIndex = 0;
    private readonly int assetBufferSize = 5;

    private readonly Vector2[] assetStartPositions = { Vector2.zero, Vector2.up * 3, Vector2.right * 3, Vector2.down * 3, Vector2.left * 3 };
    private readonly float[] assetStartRotationAngles = { 0, 0, -90, 180, 90};

    private Camera assetFollowCamera;

    private GameState previousGameState = GameState.WaitingToJoin;
    private bool gamePlayPaused = true;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;

        playerIndexThatOwnsThisBaseCamp = GetComponent<PlayerInput>().playerIndex;
        assetFollowCamera = GetComponentInChildren<Camera>();
        var maskNameToUse = (playerIndexThatOwnsThisBaseCamp == 0) ? "OnlyForPlayerOne" : "OnlyForPlayerTwo";

        // Make camera so it only shows their own land mines and not the other players
        assetFollowCamera.cullingMask |= (1 << LayerMask.NameToLayer(maskNameToUse));

        InstantiateListOfBaseCampPrefabs(playerIndexThatOwnsThisBaseCamp == 0 ? player1AssetPrefabs : player2AssetPrefabs);

        // Set BaseCamp position
        transform.position =
            playerIndexThatOwnsThisBaseCamp == 0 ?
            GridOverLordBattleFieldManager.GetPlayerOneBaseCampPosition() :
            GridOverLordBattleFieldManager.GetPlayerTwoBaseCampPosition();

        if (playerIndexThatOwnsThisBaseCamp == 0)
            EventManager.StartPlayerOneJoinedEvent();
        if (playerIndexThatOwnsThisBaseCamp == 1)
            EventManager.StartPlayerTwoJoinedEvent();
    }

    private void InstantiateListOfBaseCampPrefabs(GameObject[] arrayOfBaseCampPrefabs)
    {
        int index = 0;
        foreach (var assetPrefab in arrayOfBaseCampPrefabs)
        {
            var newInstantiatedGameObject = InstantiateAndPositionBaseCampAsset(assetPrefab, index);
            playerAssetGameObjects[index] = newInstantiatedGameObject;
            playerAssetControllerScipt[index] = newInstantiatedGameObject.GetComponentInChildren<AssetController>();
            playerAssetControllerScipt[index].InitializeAssetForBaseCamp(playerIndexThatOwnsThisBaseCamp, wallInventory, mineInventory);
            index++;
        }

        if (playerIndexThatOwnsThisBaseCamp == 1)
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
    }

    private GameObject InstantiateAndPositionBaseCampAsset(GameObject assetPrefab, int assetIndex)
    {
        var newInstantiatedGameObject = Instantiate(assetPrefab,
                (Vector3)assetStartPositions[assetIndex],
                Quaternion.identity,
                gameObject.transform);
        var baseCampAssetGameObject = new List<GameObject>
            (GameObject.FindGameObjectsWithTag("BaseCampAsset")).
            Find(g => g.transform.IsChildOf(newInstantiatedGameObject.transform));
        baseCampAssetGameObject.transform.rotation = Quaternion.AngleAxis(assetStartRotationAngles[assetIndex], Vector3.forward);
        return newInstantiatedGameObject;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void Start()
    {
        currentAssetBeingControlledIndex = 0;
        SelectThisAssetIndex(currentAssetBeingControlledIndex);
        displayPanel.GetComponent<DisplayPanelUpdates>().InitializeDisplayPanel(wallInventory, mineInventory);        
    }

    public void StartNewBattle()
    {
        wallInventory = 30;
        mineInventory = 15;

        int index = 0;
        foreach (var baseCampAssetGameObject in playerAssetGameObjects)
        {            
            Destroy(baseCampAssetGameObject);
            index++;
        }

        transform.position = Vector3.zero;

        InstantiateListOfBaseCampPrefabs(playerIndexThatOwnsThisBaseCamp == 0 ? player1AssetPrefabs : player2AssetPrefabs);

        // Set BaseCamp position
        transform.position =
            playerIndexThatOwnsThisBaseCamp == 0 ?
            GridOverLordBattleFieldManager.GetPlayerOneBaseCampPosition() :
            GridOverLordBattleFieldManager.GetPlayerTwoBaseCampPosition();

        SelectThisAssetIndex(0);
        displayPanel.GetComponent<DisplayPanelUpdates>().InitializeDisplayPanel(wallInventory, mineInventory);

        if (playerIndexThatOwnsThisBaseCamp == 1)   // just do this once - while player two is being setup
            GridOverLordBattleFieldManager.NeutralizeOilDrips( purpleOilDrip, greenOilDrip, neutralOilDrip);

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
        playerAssetControllerScipt[0].AssetRemoteControlEngaged(false);
        playerAssetControllerScipt[1].AssetRemoteControlEngaged(false);
        playerAssetControllerScipt[2].AssetRemoteControlEngaged(false);
        playerAssetControllerScipt[3].AssetRemoteControlEngaged(false);
        playerAssetControllerScipt[4].AssetRemoteControlEngaged(false);
                
        currentAssetBeingControlledIndex = newAssetIndex;
        // Select new Asset
        playerAssetControllerScipt[currentAssetBeingControlledIndex].AssetRemoteControlEngaged(true);
        assetFollowCamera.GetComponentInChildren<CameraFollow>().
            SetAssetToFollow(playerAssetGameObjects[currentAssetBeingControlledIndex].transform.GetChild(0).gameObject);

        if (playerIndexThatOwnsThisBaseCamp == 0)
            EventManager.StartPlayerOneAssetChangedEvent();
        if (playerIndexThatOwnsThisBaseCamp == 1)
            EventManager.StartPlayerTwoAssetChangedEvent();
    }

    public float getHealthOfCurrentAsset()
    {
        return (float)playerAssetControllerScipt[currentAssetBeingControlledIndex].getHealth();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (gamePlayPaused)
            return;
        playerAssetControllerScipt[currentAssetBeingControlledIndex].MoveButtonPressed(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (gamePlayPaused)
            return;
        playerAssetControllerScipt[currentAssetBeingControlledIndex].LookButtonPressed(context.ReadValue<Vector2>());
    }

    public void OnLookZeroLock(InputAction.CallbackContext context)
    {
        if (gamePlayPaused)
            return;
        playerAssetControllerScipt[currentAssetBeingControlledIndex].LookButtonZeroLockPressed(context.action.triggered);
    }


    public void OnShootButtonPressed(InputAction.CallbackContext context)
    {
        if (gamePlayPaused)
            return;
        playerAssetControllerScipt[currentAssetBeingControlledIndex].ShootButtonPressed(context.action.triggered);
    }

    public void OnBackExitKeyPressed(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            Debug.Log("Back Button Pressed");
            GameManager.OnBackArrow();
        }
    }

    public void OnOkContinueKeyPressed(InputAction.CallbackContext context)
    {
        if (!gamePlayPaused)
            return;
        if (context.action.triggered)
        {
            Debug.Log("Next Button Pressed");
            GameManager.OnOkContinue();
        }
    }

    public void OnDropWallPressed(InputAction.CallbackContext context)
    {
        if (gamePlayPaused)
            return;
        playerAssetControllerScipt[currentAssetBeingControlledIndex].DropWallButtonPressed(context.action.triggered);
    }

    public void OnDropMinePressed(InputAction.CallbackContext context)
    {
        if (gamePlayPaused)
            return;
        playerAssetControllerScipt[currentAssetBeingControlledIndex].DropMineButtonPressed(context.action.triggered);
    }

    public void OnSelectNextAsset(InputAction.CallbackContext context)
    {
        if (gamePlayPaused)
            return;
        if (context.phase == InputActionPhase.Performed)
            SelectNextActiveAsset(1);
    }

    public void OnSelectPreviousAsset(InputAction.CallbackContext context)
    {
        if (gamePlayPaused)
            return;
        if (context.phase == InputActionPhase.Performed)
            SelectNextActiveAsset(-1);
    }

    private void GameManagerOnGameStateChanged(GameState state)
    {
        gamePlayPaused = state != GameState.Play;
        if ((previousGameState == GameState.ScoreBoard) && (state == GameState.Play))
            StartNewBattle();
        previousGameState = state;  
    }
 }
