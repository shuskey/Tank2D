using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UIElements;

//
//  From https://www.youtube.com/watch?v=mbzXIOKZurA
//

public class TankController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;  // amount in a second
    [SerializeField] private float rotationSpeed = 360.0f;  // amount in a second
    [SerializeField] private float moveDistance = 3.0f;  // amount to move

    [SerializeField] private TrackController leftTrack;
    [SerializeField] private TrackController rightTrack;
    [SerializeField] private Transform movePoint;
    [SerializeField] private LayerMask whatStopsMovement;

    private Vector3 tankDirection = Vector3.up;
    private Quaternion targetRotation = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        if (isLocationApproximatlyEqual(transform.position, movePoint.position) &&
            isRotationApproximatlyEqual(transform.rotation, targetRotation))
        {
            startTracks(false);

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

                if (delta == 0 || delta == -360)  // just do a move - we are already poining this way
                {
                    var potentialMovePoint = movePoint.position + transform.up * moveDistance;
                    if (!Physics2D.OverlapCircle(potentialMovePoint, 0.2f, whatStopsMovement))
                        movePoint.position = potentialMovePoint;
                } else if (delta == 180 || delta == -180)  //  Beep beep beep - we are backing up
                {
                    var potentialMovePoint = movePoint.position - transform.up * moveDistance;
                    if (!Physics2D.OverlapCircle(potentialMovePoint, 0.2f, whatStopsMovement))
                        movePoint.position = potentialMovePoint;
                } else if (delta == 90 || delta == -270)  // rotate
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
        leftTrack.animator.SetBool("moving", start);
        rightTrack.animator.SetBool("moving", start);
    }
}
