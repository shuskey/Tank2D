using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class SpriteBlink : MonoBehaviour
{
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = new Color(1, 1, 1, 0);
    [Range(0, 10)] public float speed = 10f;
    private bool isBlinking = false;

    Image imageComponentToBlink;
    void Awake()
    {
        imageComponentToBlink = GetComponent<Image>();
    }

    private void Update()
    {


    }

    public void blinkOnce()
    {
        if (!isBlinking)
            StartCoroutine(BlinkRoutine());

    }

    private IEnumerator BlinkRoutine()
    {
        float timeSpentHere = 0; 
        isBlinking = true;

        while (timeSpentHere < 0.25)
        {
            // this looks scary but is fine in a Coroutine
            // as long as you YIELD somewhere inside!
            imageComponentToBlink.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
            yield return null;
            timeSpentHere += Time.deltaTime;
        }
        imageComponentToBlink.color = startColor;
        isBlinking = false; 
    }
}
