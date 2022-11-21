using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class WinLoseVideoClipPlayer : MonoBehaviour
{
    [SerializeField] private VideoClip[] loserClips; 
    [SerializeField] private VideoClip[] winnerClips;
    [SerializeField] bool playWinningClips;
    [SerializeField] private RenderTexture playerOneTexture, playerTwoTexture;

    private VideoPlayer videoPlayer;
    private RawImage rawImage;  // place where the video is rendered
    private int numberOfLoserClips, numberOfWinnerClips;

    
    private void Awake()
    {
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        rawImage = GetComponentInChildren<RawImage>();
        numberOfLoserClips = loserClips.Length;
        numberOfWinnerClips = winnerClips.Length;
    }

    public void SetPlayerOneTexture()
    {
        videoPlayer.targetTexture = playerOneTexture;
        rawImage.texture = playerOneTexture;
    }

    public void SetPlayerTwoTexture()
    {
        videoPlayer.targetTexture = playerTwoTexture;
        rawImage.texture = playerTwoTexture;
    }

    public void SetWinningVideoToPlay()
    {
        videoPlayer.clip = randomWinnerClip();
        videoPlayer.Play();
        var color = rawImage.color; color.a = 1; rawImage.color = color; rawImage.CrossFadeAlpha(1, 0.5f, true);
    }

    public void SetLosingVideoToPlay()
    {
        videoPlayer.clip = randomLoserClip();
        videoPlayer.Play();
        var color = rawImage.color;  color.a = 1;  rawImage.color = color;
    }

    public void SetVideoToStop()
    {
        var color = rawImage.color; color.a = 0; rawImage.color = color;
        videoPlayer?.Stop();
    }

    private VideoClip randomWinnerClip() => winnerClips[Random.Range(0, numberOfWinnerClips)];

    private VideoClip randomLoserClip() => loserClips[Random.Range(0, numberOfLoserClips)];
}
