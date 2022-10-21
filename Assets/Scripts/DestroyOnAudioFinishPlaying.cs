using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAudioFinishPlaying : MonoBehaviour
{
    AudioSource audioSource; 

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(gameObject);
    }
}
