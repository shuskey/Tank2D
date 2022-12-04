using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaDisolveOverTime : MonoBehaviour
{
    private float timeToLive = 2.0f;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();        
    }

    private void OnDisable()
    {
       // StartCoroutine(SpriteFade(timeToLive));
    }

    private void OnEnable()
    {
        StartCoroutine(SpriteFade(timeToLive)); 
    }

    IEnumerator SpriteFade(float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
            yield return null;
        }
    }
}
