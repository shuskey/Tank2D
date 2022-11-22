using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpdateScoreBoard : MonoBehaviour
{
    [SerializeField] private TMP_Text myScoreText;
    [SerializeField] private TMP_Text enemyScoreText;
    [SerializeField] private GameObject myTankSpritePanel;
    [SerializeField] private GameObject ememyTankSpritePanel;
    [SerializeField] private Sprite purpleTankSprite;
    [SerializeField] private Sprite greenTankSprite;

    private Canvas myCanvas;

    private WinLoseVideoClipPlayer winLoseVideoClipPlayer;

    private void Awake()
    {
        winLoseVideoClipPlayer = GetComponentInChildren<WinLoseVideoClipPlayer>();
        myCanvas = GetComponentInParent<Canvas>();
        if (myCanvas.worldCamera.targetDisplay == 0) //Player 1
        {
            myTankSpritePanel.GetComponent<Image>().sprite = purpleTankSprite;
            ememyTankSpritePanel.GetComponent<Image>().sprite = greenTankSprite;
        }
        else
        {
            myTankSpritePanel.GetComponent<Image>().sprite = greenTankSprite;
            ememyTankSpritePanel.GetComponent<Image>().sprite = purpleTankSprite;
        }
    }

    private void Start()
    {

    }

    public void UpdateTheScore()
    {
        if (myCanvas.worldCamera.targetDisplay == 0)  // Player 1
        {
            myScoreText.text = GameManager.playerOne.wins.ToString();
            enemyScoreText.text = GameManager.playerTwo.wins.ToString();
            CheckForWinnerLoserVideoTime(GameManager.playerOne.wins, GameManager.playerTwo.wins);
        }
        else
        {
            myScoreText.text = GameManager.playerTwo.wins.ToString();
            enemyScoreText.text = GameManager.playerOne.wins.ToString();
            CheckForWinnerLoserVideoTime(GameManager.playerTwo.wins, GameManager.playerOne.wins);
        }        
    }

    public void CloseDownResourcesInScoreBoard()
    {
        if (winLoseVideoClipPlayer != null)
            winLoseVideoClipPlayer.SetVideoToStop();
    }


    public void CheckForWinnerLoserVideoTime(int myScore, int enemyScore)
    {
        if (myScore + enemyScore == 7)  // best of Seven
        {            
            if (myCanvas.worldCamera.targetDisplay == 0) //Player 1
            {
                winLoseVideoClipPlayer.SetPlayerOneTexture();
            }
            else
            {
                winLoseVideoClipPlayer.SetPlayerTwoTexture();
            }

            if (myScore > enemyScore)
                winLoseVideoClipPlayer.SetWinningVideoToPlay();
            else
                winLoseVideoClipPlayer.SetLosingVideoToPlay();

            GameManager.OnTheGameIsOver();
        }

    }
}
