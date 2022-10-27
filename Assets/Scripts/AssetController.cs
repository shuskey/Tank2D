using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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

    public UnityEvent<bool> OnShoot;

    private Vector3 tankDirection = Vector3.up;
    private Quaternion targetRotation = Quaternion.identity;
    private Vector2 movementInput = Vector2.zero;
    private bool assetEngaged = false;

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
        myLittleLightGameObject.SetActive(false);
    }

    public void AssetRemoteControlEngaged(bool assetEngaged)
    {
        this.assetEngaged = assetEngaged;
        myLittleLightGameObject.SetActive(assetEngaged);
    }

    public void MoveButtonPressed(Vector2 moveVector)
    {     
        movementInput = moveVector;
    }

    public void ShootButtonPressed(bool shootButtonState)
    {        
        OnShoot?.Invoke(shootButtonState);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        var horizontal = removeDeadZone(movementInput.x); // Input.GetAxisRaw("Horizontal");
        var vertical = removeDeadZone(movementInput.y); // Input.GetAxisRaw("Vertical");

        if (isLocationApproximatlyEqual(transform.position, movePoint.position) &&
            isRotationApproximatlyEqual(transform.rotation, targetRotation))
        {
            startTracks(false);
           // Debug.Log($"Input Movement = {horizontal} {vertical} {movementInput.x} {movementInput.y}");

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
                var delta = requestedInputAngle - targetRotation.eulerAngles.z;                

                if ((delta == 0 || delta == -360) &&                          // just do a move - we are already poining this way
                    (leftTrack is not null && rightTrack is not null))      // some assets have NO wheels/track so do not move them
                    {
                    var potentialMovePoint = movePoint.position + transform.up * moveDistance;
                    if (!Physics2D.OverlapCircle(potentialMovePoint, 0.2f, whatStopsMovement))
                        movePoint.position = potentialMovePoint;
                } else if (delta == 180 || delta == -180)  
                {
                    // lets just start the turn around instead of backing up
                    targetRotation *= Quaternion.AngleAxis(90, Vector3.forward);
                    //  Beep beep beep - we are backing up
                    //var potentialMovePoint = movePoint.position - transform.up * moveDistance;
                    //if (!Physics2D.OverlapCircle(potentialMovePoint, 0.2f, whatStopsMovement))
                    //    movePoint.position = potentialMovePoint;
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
        float deadZoneMin = 0.125f;
        float deadZoneMax = 0.925f;
        var absInput = MathF.Abs(input);
        return Mathf.Sign(input) * (absInput > deadZoneMax ? 1.0f : absInput < deadZoneMin ? 0 : absInput);
    }
}
