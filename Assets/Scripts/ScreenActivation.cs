using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenActivation : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("ScreenActivation");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var disp in Display.displays)
        {
            disp.Activate(disp.systemWidth, disp.systemHeight, 60);
        }        
    }
}
