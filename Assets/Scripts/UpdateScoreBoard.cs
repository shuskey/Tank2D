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

    private void Awake()
    {
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateTheScore()
    {
        if (myCanvas.worldCamera.targetDisplay == 0)  // Player 1
        {
            myScoreText.text = GameManager.playerOneScore.ToString();
            enemyScoreText.text = GameManager.playerTwoScore.ToString();
        }
        else
        {
            myScoreText.text = GameManager.playerTwoScore.ToString();
            enemyScoreText.text = GameManager.playerOneScore.ToString();
        }
    }
}
