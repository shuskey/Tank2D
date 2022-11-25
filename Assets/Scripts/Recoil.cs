using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{

    [SerializeField] private float maximumOffestDistance;
    [SerializeField] private float recoilAcceleration;
    [SerializeField] private float weaponRecoilStartSpeed;

    private bool recoilInEffect;
    private bool weaponHeadedBackToStartPosition;
    private Vector3 offsetPosition;
    private Vector3 recoilSpeed;

    public void AddRecoil()
    {
        recoilInEffect = true;
        weaponHeadedBackToStartPosition = false;
        recoilSpeed = Vector3.up * weaponRecoilStartSpeed;
    }

    private void Start()
    {
        recoilSpeed = Vector3.zero;
        offsetPosition = Vector3.zero;

        recoilInEffect = false;
        weaponHeadedBackToStartPosition = false;        
    }

    private void Update()
    {
        UpdateRecoil();
    }

    private void UpdateRecoil()
    {
        if (!recoilInEffect) return;

        // setup speed and position
        recoilSpeed += (-offsetPosition.normalized) * recoilAcceleration * Time.deltaTime;
        Vector3 newOffsetPostion = offsetPosition + recoilSpeed * Time.deltaTime;
        Vector3 newTransformPosition = transform.localPosition - offsetPosition;

        if (newOffsetPostion.magnitude > maximumOffestDistance)
        {
            recoilSpeed = Vector3.zero;
            weaponHeadedBackToStartPosition = true;
            newOffsetPostion = offsetPosition.normalized * maximumOffestDistance;
        }
        else if (weaponHeadedBackToStartPosition && newOffsetPostion.magnitude > offsetPosition.magnitude)
        {
            transform.localPosition -= offsetPosition;
            offsetPosition = Vector3.zero;
            recoilInEffect = false;
            weaponHeadedBackToStartPosition = false;
            return;
        }
        transform.localPosition = newTransformPosition + newOffsetPostion;
        offsetPosition = newOffsetPostion;
    }
}
