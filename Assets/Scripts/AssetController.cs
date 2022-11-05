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


public class AssetController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;  // amount in a second
    [SerializeField] private float rotationSpeed = 360.0f;  // amount in a second
    [SerializeField] private float moveDistance = 3.0f;  // amount to move

    [SerializeField] private TrackController leftTrack;
    [SerializeField] private TrackController rightTrack;
    [SerializeField] private GameObject myLittleLightGameObject;
    [SerializeField] private Transform movePoint;
    [SerializeField] private LayerMask whatStopsMovement;
    [SerializeField] private LayerMask playerOneMine;
    [SerializeField] private LayerMask playerTwoMine;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject minePrefab;

    private GameObject thingToDropPrefab;

    public UnityEvent<bool> OnShoot;
    public UnityEvent<bool> OnSelfdestruct;

    private string gridLayerGroundOverlay = "Ground Overlay";
    private string gridLayerInFrontOfPlayer = "In Front of Player";
    private string gridLayerColliders = "Colliders";

    private Vector3 tankDirection = Vector3.up;
    private Quaternion targetRotation = Quaternion.identity;
    private Vector2 movementInput = Vector2.zero;
    private bool assetEngaged = false;
    private bool iAmABaseOnlyMoveGunTurret = false;    
    private GameObject gunTurretGameObject;
    private int playerIndexThatIBelongTo;
    private Vector3 previousAssetLocation;
    private bool dropThingWhileMoving = false;
    private bool weAreMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        // Only search Children 
        gunTurretGameObject = new List<GameObject>
            (GameObject.FindGameObjectsWithTag("GunTurret")).
            Find(g => g.transform.IsChildOf(this.transform));        
        iAmABaseOnlyMoveGunTurret = gunTurretGameObject == null ? false : true;

        movePoint.parent = null;
        myLittleLightGameObject.SetActive(false);

        // Your target rotation start with your current orientation
        targetRotation = transform.localRotation;
        previousAssetLocation = transform.position;
    }
        
    private void DropThingAtLocation(Vector3 dropLocation)
    {        
        Tilemap tilemap = GameObject.FindObjectsOfType<Tilemap>().
            Where<Tilemap>(i => i.name == gridLayerInFrontOfPlayer).FirstOrDefault();
        var currentTileCellCoordinates = tilemap.WorldToCell(dropLocation);
        tilemap.SetTile(currentTileCellCoordinates, null);  // delete anything OVER the player

        var newInstantiatedGameObject = Instantiate(thingToDropPrefab, dropLocation, Quaternion.identity);            
    }

    //private bool AreWeSittingOnALandMine(Vector3 sittingLocation)
    //{
    //    if (Physics2D.OverlapCircle(sittingLocation, 0.2f, playerOneMine) ||
    //        Physics2D.OverlapCircle(sittingLocation, 0.2f, playerOneMine))
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    public void ListenForBaseCampDestruction(int playerIndex)
    {
        playerIndexThatIBelongTo = playerIndex;        
    }

    private void Awake()
    { 
        // Subscribe to Events
        EventManager.PlayerOneBaseCampDefeatedEvent += BaseCampOneDefeated;
        EventManager.PlayerTwoBaseCampDefeatedEvent += BaseCampTwoDefeated;
    }

    private void OnDestroy()
    {
        EventManager.PlayerOneBaseCampDefeatedEvent -= BaseCampOneDefeated;
        EventManager.PlayerTwoBaseCampDefeatedEvent -= BaseCampTwoDefeated;
    }
    private void BaseCampOneDefeated()
    {
        // If this was our Base Camp, then self distruct
        SelfDestruct(0);
    }

    private void BaseCampTwoDefeated()
    {
        // If this was our Base Camp, then self distruct
        SelfDestruct(1);
    }

    private void SelfDestruct(int playerIndexThatWasDefeated)
    {
        if (playerIndexThatIBelongTo == playerIndexThatWasDefeated)
        {
            OnSelfdestruct?.Invoke(true);
        }
    }

    public void AssetRemoteControlEngaged(bool assetEngaged)
    {
        this.assetEngaged = assetEngaged;
        myLittleLightGameObject.SetActive(assetEngaged);
    }

    public void MoveButtonPressed(Vector2 moveVector)
    {

        if (dropThingWhileMoving)
        {
            return;
        }
        movementInput = moveVector;
    }

    public void ShootButtonPressed(bool shootButtonState)
    {        
        OnShoot?.Invoke(shootButtonState);
    }

    public void DropWallButtonPressed(bool keyPressOrRelease)
    {
        // try and move the tank forward (the direction it is currently pointed)
        // If that works
        // put a Wall in the space we were just in

        // true: keyPress and not already dropping a wall
        // false: keyRelease


        if (keyPressOrRelease)
        {            
            if (weAreMoving)
                return;

            var angle = transform.localRotation.eulerAngles.z;

            movementInput = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up;            
            thingToDropPrefab = wallPrefab;
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
        if (keyPressOrRelease)
        {
            if (weAreMoving)         
                return;
         
            var angle = transform.localRotation.eulerAngles.z;

            movementInput = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up;            
            thingToDropPrefab = minePrefab;
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
        float horizontal;
        float vertical;

        if (iAmABaseOnlyMoveGunTurret)
        {            
            if (movementInput != Vector2.zero)
            {
                float targetRotationAngle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg - 90;
                var q = Quaternion.AngleAxis(targetRotationAngle, Vector3.forward);
                gunTurretGameObject.transform.rotation = 
                    Quaternion.Slerp(gunTurretGameObject.transform.rotation, q, rotationSpeed * Time.deltaTime);
                
            }
            return;
        }

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

            //if (AreWeSittingOnALandMine(movePoint.position))
            //{
            //    var damagable = GetComponent<Damagable>();
            //    if (damagable != null)
            //        damagable.Hit(50);
            //}

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
                    if (Physics2D.OverlapCircle(potentialMovePoint, 0.2f, whatStopsMovement))
                    {
                        dropThingWhileMoving = false;
                        weAreMoving = false;
                    } else {
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

    private bool isAngleApproximatlyEqual(float a1, float a2, float precision = 1.0f) =>
    Mathf.Abs(a2 - a1) <= precision;

    private bool isRotationApproximatlyEqual(Quaternion q1, Quaternion q2, float precision = 0.0000004f) => 
        Mathf.Abs(Quaternion.Dot(q1, q2)) >= 1 - precision;

    private bool isLocationApproximatlyEqual(Vector3 p1, Vector3 p2, float percision = 0.05f) => 
        Vector3.Distance(transform.position, movePoint.position) < percision;

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
