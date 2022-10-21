using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateUtility : MonoBehaviour
{
    public GameObject objectToInstatiate;

    public void InstantiateObject()
    {
        Instantiate(objectToInstatiate);
    }
}
