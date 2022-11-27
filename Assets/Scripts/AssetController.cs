using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static GameManager;
using Random = UnityEngine.Random;

public class AssetController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;  // amount in a second
    [SerializeField] private float rotationSpeed = 360.0f;  // amount in a second
    [SerializeField] private float moveDistance = 3.0f;  // amount to move
    [SerializeField] private Camera baseCamera;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private TrackController leftTrack;
    [SerializeField] private TrackController rightTrack;
    [SerializeField] private GameObject myLittleLightGameObject;
    [SerializeField] private Transform movePoint;
    [SerializeField] private LayerMask whatStopsMovement;
    [SerializeField] private LayerMask playerOneMine;
    [SerializeField] private LayerMask playerTwoMine;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private Tile oilDripTile;


    private bool gamePlayPaused = false;

    private int wallInventoryForBaseCamp = 10;
    private int mineInventoryForBaseCamp = 10;    

    public UnityEvent<bool> OnShoot;
    public UnityEvent<bool> OnSelfdestruct;

    private string gridLayerGroundOverlay = "Ground Overlay";
    private string gridLayerInFrontOfPlayer = "In Front of Player";
    private string gridLayerColliders = "Colliders";

    private Vector3 tankDirection = Vector3.up;
    private Quaternion targetRotation = Quaternion.identity;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    private bool assetEngaged = false;
    private bool iAmABaseOnlyMoveGunTurret = false;    
    private GameObject gunTurretGameObject;
    private int playerIndexThatIBelongTo;
    private Vector3 previousAssetLocation;
    private bool dropThingWhileMoving = false;    
    private WhatToDrop whatToDrop = WhatToDrop.Nothing;
    private bool weAreMoving = false;

    private enum WhatToDrop
    {
        Nothing,
        Wall,
        Mine
    }

    // Start is called before the first frame update
    void Start()
    {
        StartUpAsset();
    }

    public void StartUpAsset()
    {
        if (gamePlayPaused)
        {
            if (baseCamera != null)
                baseCamera.enabled = false;
            if (healthBar != null)
                healthBar.SetActive(false);
        }

        // Only search Children 
        gunTurretGameObject = new List<GameObject>
            (GameObject.FindGameObjectsWithTag("GunTurret")).
            Find(g => g.transform.IsChildOf(this.transform));
        iAmABaseOnlyMoveGunTurret = gameObject.name.ToLower().Contains("base");
        movePoint.parent = null;        

        // Your target rotation start with your current orientation
        targetRotation = transform.localRotation;
        previousAssetLocation = transform.position;
    }

    public void ReAttachMovePoint()
    {
        movePoint.parent = gameObject.transform;
        movePoint.transform.localPosition = Vector3.zero;
    }
      
    public int playerIndex => playerIndexThatIBelongTo;
   
    public bool isThisTheEngagedAsset => assetEngaged;

    private void OilDripsWhileMoving(Vector3 dripLocation)
    {
        Tilemap tilemap = GameObject.FindObjectsOfType<Tilemap>().Where<Tilemap>(i => i.name == gridLayerGroundOverlay).FirstOrDefault();
        var currentTileCellCoordinates = tilemap.WorldToCell(dripLocation);
        tilemap.SetTile(currentTileCellCoordinates, oilDripTile);
    }

    private void DropThingAtLocation(Vector3 dropLocation)
    {
        if (whatToDrop == WhatToDrop.Nothing)
            return;

        Tilemap tilemap = GameObject.FindObjectsOfType<Tilemap>().
            Where<Tilemap>(i => i.name == gridLayerInFrontOfPlayer).FirstOrDefault();
        var currentTileCellCoordinates = tilemap.WorldToCell(dropLocation);
        tilemap.SetTile(currentTileCellCoordinates, null);  // delete anything OVER the player        
        
        if (whatToDrop == WhatToDrop.Wall)
        {            
            Instantiate(wallPrefab, dropLocation, Quaternion.identity);

            if (thisIsMyPlayer(0))
                EventManager.StartPlayerOneWallDeployedEvent();
            else
                EventManager.StartPlayerTwoWallDeployedEvent();
        }
        if (whatToDrop == WhatToDrop.Mine)
        {
            Instantiate(minePrefab, dropLocation, Quaternion.identity);

            if (thisIsMyPlayer(0))
                EventManager.StartPlayerOneMineDeployedEvent();
            else
                EventManager.StartPlayerTwoMineDeployedEvent();
        }
    }

    public void InitializeAssetForBaseCamp(int playerIndex, int initialWallInventory, int initialMineInventory)
    {
        playerIndexThatIBelongTo = playerIndex;        
        wallInventoryForBaseCamp = initialWallInventory;
        mineInventoryForBaseCamp = initialMineInventory;
        gameObject.GetComponentInChildren<Damagable>().RestoreAllHealth();
        gameObject.transform.parent.gameObject.SetActive(true);  // Revive
        movePoint.position = gameObject.transform.position;       
    }

    private void Awake()
    { 
        // Subscribe to Events
        EventManager.PlayerOneBaseCampDefeatedEvent += BaseCampOneDefeated;
        EventManager.PlayerTwoBaseCampDefeatedEvent += BaseCampTwoDefeated;
        EventManager.PlayerOneWallDeployedEvent += PlayerOneWallDeployed;
        EventManager.PlayerTwoWallDeployedEvent += PlayerTwoWallDeployed;
        EventManager.PlayerOneMineDeployedEvent += PlayerOneMineDeployed;
        EventManager.PlayerTwoMineDeployedEvent += PlayerTwoMineDeployed;
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;

        myLittleLightGameObject.SetActive(false);

    }

    private void OnDestroy()
    {
        EventManager.PlayerOneBaseCampDefeatedEvent -= BaseCampOneDefeated;
        EventManager.PlayerTwoBaseCampDefeatedEvent -= BaseCampTwoDefeated;
        EventManager.PlayerOneWallDeployedEvent -= PlayerOneWallDeployed;
        EventManager.PlayerTwoWallDeployedEvent -= PlayerTwoWallDeployed;
        EventManager.PlayerOneMineDeployedEvent -= PlayerOneMineDeployed;
        EventManager.PlayerTwoMineDeployedEvent -= PlayerTwoMineDeployed;
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;

        if (movePoint != null)
            Destroy(movePoint.gameObject);
    }

    private void BaseCampOneDefeated() => SelfDestruct(0);
    private void BaseCampTwoDefeated() => SelfDestruct(1);
    private void PlayerOneWallDeployed() => WallDecrement(0);
    private void PlayerTwoWallDeployed() => WallDecrement(1);
    private void PlayerOneMineDeployed() => MineDecrement(0);
    private void PlayerTwoMineDeployed() => MineDecrement(1);
    private void WallDecrement(int playerIndex) { GameManager.WallsDroped(playerIndex); if (thisIsMyPlayer(playerIndex)) { wallInventoryForBaseCamp--; } }
    private void MineDecrement(int playerIndex) { GameManager.MineDroped(playerIndex); if (thisIsMyPlayer(playerIndex)) { mineInventoryForBaseCamp--; } }
    private void SelfDestruct(int playerIndex) {
        if (thisIsMyPlayer(playerIndex)) 
        { 
            OnSelfdestruct?.Invoke(true);            
        } 
    }

    private bool thisIsMyPlayer(int playerIndex) => (playerIndexThatIBelongTo == playerIndex);

    private void GameManagerOnGameStateChanged(GameState state)
    {
        gamePlayPaused = state != GameState.Play;
        if (baseCamera != null)
            baseCamera.enabled = !gamePlayPaused;
        if (healthBar != null)
            healthBar.SetActive(!gamePlayPaused);
    }

    public void AssetRemoteControlEngaged(bool assetEngaged)
    {
        //Stop any activies that might be in progress
        MoveButtonPressed(Vector2.zero);
        ShootButtonPressed(false);

        this.assetEngaged = assetEngaged;
        myLittleLightGameObject.SetActive(assetEngaged);
    }

    public void MoveButtonPressed(Vector2 moveVector)
    {
        if (gamePlayPaused)
            return;
        if (!dropThingWhileMoving)
        {
            movementInput = moveVector;
            previousAssetLocation = movePoint.position;            
        }
    }

    public void LookButtonPressed(Vector2 moveVector)
    {
        if (gamePlayPaused)
            return;
        
        lookInput = moveVector;            
    }

    public void ShootButtonPressed(bool shootButtonState)
    {
        if (gamePlayPaused)
            return;
        OnShoot?.Invoke(shootButtonState);
    }

    public void DropWallButtonPressed(bool keyPressOrRelease)
    {
        if (gamePlayPaused)
            return;
        if (keyPressOrRelease && wallInventoryForBaseCamp > 0)
        {            
            if (weAreMoving)
                return;

            var angle = transform.localRotation.eulerAngles.z;

            movementInput = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up;
            whatToDrop = WhatToDrop.Wall;
            dropThingWhileMoving = true;
            previousAssetLocation = movePoint.position;            
        }
        else
        {         
            movementInput = Vector2.zero;
        }        
    }

    public void DropMineButtonPressed(bool keyPressOrRelease)
    {
        if (gamePlayPaused)
            return;
        if (keyPressOrRelease && mineInventoryForBaseCamp > 0)
        {
            if (weAreMoving)         
                return;
         
            var angle = transform.localRotation.eulerAngles.z;

            movementInput = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up;
            whatToDrop = WhatToDrop.Mine;
            dropThingWhileMoving = true;
            previousAssetLocation = movePoint.position;            
        }
        else
        {
            movementInput = Vector2.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePlayPaused)
            return;
        float horizontal;
        float vertical;

        if (lookInput != Vector2.zero)
        {
            float targetRotationAngle = Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg - 90;
            var q = Quaternion.AngleAxis(targetRotationAngle, Vector3.forward);
            gunTurretGameObject.transform.rotation = 
                Quaternion.Slerp(gunTurretGameObject.transform.rotation, q, rotationSpeed * Time.deltaTime);
                
        }

        if (iAmABaseOnlyMoveGunTurret) return;
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (isLocationApproximatlyEqual(transform.position, movePoint.position) &&
            isRotationApproximatlyEqual(transform.rotation, targetRotation))
        {            

            startTracks(false);
            if (dropThingWhileMoving && weAreMoving)
            {
                dropThingWhileMoving = false;                
                DropThingAtLocation(previousAssetLocation);
                //Stop
                horizontal = vertical = 0f;
                movementInput = Vector2.zero;
            }
            else
            {
                horizontal = removeDeadZone(movementInput.x); // Input.GetAxisRaw("Horizontal");
                vertical = removeDeadZone(movementInput.y); // Input.GetAxisRaw("Vertical");
            }
            weAreMoving = false;

            if ((Mathf.Abs(horizontal) == 1 || Mathf.Abs(vertical) == 1))  // if we have some input
            {
                // Get the requested Angle based on the Input just received
                var requestedInputAngle = 0.0f;
                if (Mathf.Abs(horizontal) == 1)   // result in -90 ro 90
                    requestedInputAngle = Mathf.Sign(horizontal) * -90;
                // No diagonals please
                if (Mathf.Abs(vertical) == 1)    // result in 0 or 180
                    requestedInputAngle = Mathf.Sign(vertical) * -90 + 90;

                // Find the difference (delta) with the current tank rotation
                var delta = Mathf.Round(requestedInputAngle - targetRotation.eulerAngles.z); 

                if ((delta == 0 || delta == -360) &&                          // just do a move - we are already poining this way
                    (leftTrack is not null && rightTrack is not null))      // some assets have NO wheels/track so do not move them
                    {
                    var potentialMovePoint = movePoint.position + transform.up * moveDistance;
                    if (Physics2D.OverlapCircle(potentialMovePoint, 0.5f, whatStopsMovement) || DoesMovePointMatchAnyOtherMovePoint(potentialMovePoint))
                    {
                        dropThingWhileMoving = false;
                        weAreMoving = false;
                    } else {
                        OilDripsWhileMoving(movePoint.position);

                        movePoint.position = potentialMovePoint;
                        weAreMoving = true;
                    }
                } else if (delta == 180 || delta == -180)  
                {
                    // lets just start the turn around instead of backing up
                    targetRotation *= Quaternion.AngleAxis(90, Vector3.forward);
                }
                else if (delta == 90 || delta == -270)  // rotate
                {
                    targetRotation *= Quaternion.AngleAxis( 90, Vector3.forward);
                } else if (delta == -90 || delta == 270)   // rotate
                {
                    targetRotation *= Quaternion.AngleAxis( -90, Vector3.forward);
                }
            }
        }
        else
        {
            startTracks(true);
        }
    }

    private bool DoesMovePointMatchAnyOtherMovePoint(Vector3 proposedMovePoint)
    {
        var allMovePointsGameObjects = GameObject.FindGameObjectsWithTag("MovePoint");
        foreach (var otherMovePointGameObject in allMovePointsGameObjects)
        {
            if (otherMovePointGameObject.transform.position == proposedMovePoint)
            {
                return true;
            }
            else if (isLocationApproximatlyEqual(otherMovePointGameObject.transform.position, proposedMovePoint))
            {
                return true;
            }
        }
        return false;
    }

    private bool isRotationApproximatlyEqual(Quaternion q1, Quaternion q2, float precision = 0.0000004f) => 
        Mathf.Abs(Quaternion.Dot(q1, q2)) >= 1 - precision;

    private bool isLocationApproximatlyEqual(Vector3 p1, Vector3 p2, float percision = 0.05f) => 
        Vector3.Distance(p1, p2) < percision;

    void startTracks(bool start)
    {
        if (leftTrack is not null)
            leftTrack.animator.SetBool("moving", start);
        if (rightTrack is not null)
            rightTrack.animator.SetBool("moving", start);
    }

    private float removeDeadZone(float input)
    {
        float deadZoneMin = 0.285f;
        float deadZoneMax = 0.715f;
        var absInput = MathF.Abs(input);
        return Mathf.Sign(input) * (absInput > deadZoneMax ? 1.0f : absInput < deadZoneMin ? 0 : absInput);
    }
}
