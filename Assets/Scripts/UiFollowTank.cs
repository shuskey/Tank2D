using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFollowTank : MonoBehaviour
{
    [SerializeField] private Transform objectToFollow;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();        
    }

    // Update is called once per frame
    void Update()
    {
       if (objectToFollow != null)
            rectTransform.anchoredPosition = objectToFollow.localPosition;
    }
}
