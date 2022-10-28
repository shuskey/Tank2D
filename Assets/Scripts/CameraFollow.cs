using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Vector3 offset;

    public void SetAssetToFollow(GameObject assetGameObject)
    {
        player = assetGameObject;
    }

    void Start()
    {
        if (player != null)
            offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        if (player != null)
            transform.position = player.transform.position + offset;
    }
}
