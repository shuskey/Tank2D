using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private float shakeTimeRemaining;
    private float shakePower;
    private float timeBetweenShakes;

    // Start is called before the first frame update
    void Start()
    {
        timeBetweenShakes = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeBetweenShakes <= 0f)
        {
            StartShake(.5f, 1f);
            timeBetweenShakes = 5f;
        }
        timeBetweenShakes -= Time.deltaTime;        
    }

    private void LateUpdate()
    {
        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;

            float xAmount = Random.Range(-shakePower, shakePower);
            float yAmount = Random.Range(-shakePower, shakePower);

            transform.position += new Vector3(xAmount, yAmount, 0f);
        }
    }

    public void StartShake(float length, float power)
    {
        shakeTimeRemaining = length;
        shakePower = power;
    }
}
