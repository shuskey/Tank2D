using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPanelUpdates : MonoBehaviour
{

    [SerializeField] private TMP_Text topDisplayText;
    [SerializeField] private TMP_Text wallCountText;
    [SerializeField] private TMP_Text mineCountText;
    [SerializeField] private TMP_Text myWinCountText;
    [SerializeField] private TMP_Text enemyWinCountText;
    [SerializeField] private Image wallIconGoesHere;
    [SerializeField] private Image enemyTankIcon1GoesHere;
    [SerializeField] private Image enemyTankIcon2GoesHere;
    [SerializeField] private Image enemyTankIcon3GoesHere;
    [SerializeField] private Image enemyTankIcon4GoesHere;
    [SerializeField] private Image myTankIcon1GoesHere;
    [SerializeField] private Image myTankIcon2GoesHere;
    [SerializeField] private Image myTankIcon3GoesHere;
    [SerializeField] private Image myTankIcon4GoesHere;

    [SerializeField] private Sprite iconPurpleWall;
    [SerializeField] private Sprite iconGreenWall;
    [SerializeField] private Sprite iconPurpleTank;
    [SerializeField] private Sprite iconPurpleTankExploded;
    [SerializeField] private Sprite iconGreenTank;
    [SerializeField] private Sprite iconGreenTankExploded;

    private int playerIndex;
    private int playerOneTankCount = 4;
    private int playerTwoTankCount = 4;
    private int wallCount, mineCount;
    private Sprite enemyTankIcon, enemyTankExplodedIcon, myTankIcon, myTankExplodedIcon;

    // Start is called before the first frame update
    void Start()
    {
        playerIndex = GetComponentInParent<Canvas>().worldCamera.targetDisplay;

        enemyTankIcon = playerIndex == 0 ? iconGreenTank : iconPurpleTank;
        enemyTankExplodedIcon = playerIndex == 0 ? iconGreenTankExploded : iconPurpleTankExploded;
        myTankIcon = playerIndex == 0 ? iconPurpleTank : iconGreenTank;
        myTankExplodedIcon = playerIndex == 0 ? iconPurpleTankExploded : iconGreenTankExploded;                
        wallIconGoesHere.sprite = playerIndex == 0 ? iconPurpleWall : iconGreenWall;

        UpdateDisplayStatus();
    }

    private void Awake()
    {
        EventManager.CelebratePlayerOneDefeatedEvent += ShowPlayerOneDefeated;
        EventManager.CelebratePlayerTwoDefeatedEvent += ShowPlayerTwoDefeated;

        EventManager.PlayerOneTankDefeatedEvent += PlayerOneTankDefeated;
        EventManager.PlayerTwoTankDefeatedEvent += PlayerTwoTankDefeated;

        EventManager.PlayerOneTankHitEvent += PlayerOneTankHit;
        EventManager.PlayerTwoTankHitEvent += PlayerTwoTankHit;

        EventManager.PlayerOneBaseHitEvent += PlayerOneBaseHit;
        EventManager.PlayerTwoBaseHitEvent += PlayerTwoBaseHit;

        EventManager.PlayerOneAssetChangedEvent += PlayerOneAssetChanged;
        EventManager.PlayerTwoAssetChangedEvent += PlayerTwoAssetChanged;

        EventManager.PlayerOneWallDeployedEvent += PlayerOneWallDeployed;
        EventManager.PlayerTwoWallDeployedEvent += PlayerTwoWallDeployed;
        EventManager.PlayerOneMineDeployedEvent += PlayerOneMineDeployed;
        EventManager.PlayerTwoMineDeployedEvent += PlayerTwoMineDeployed;
    }

    private void OnDisable()
    {
        EventManager.CelebratePlayerOneDefeatedEvent -= ShowPlayerOneDefeated;
        EventManager.CelebratePlayerTwoDefeatedEvent -= ShowPlayerTwoDefeated;

        EventManager.PlayerOneTankDefeatedEvent -= PlayerOneTankDefeated;
        EventManager.PlayerTwoTankDefeatedEvent -= PlayerTwoTankDefeated;

        EventManager.PlayerOneTankHitEvent -= PlayerOneTankHit;
        EventManager.PlayerTwoTankHitEvent -= PlayerTwoTankHit;

        EventManager.PlayerOneWallDeployedEvent -= PlayerOneWallDeployed;
        EventManager.PlayerTwoWallDeployedEvent -= PlayerTwoWallDeployed;
        EventManager.PlayerOneMineDeployedEvent -= PlayerOneMineDeployed;
        EventManager.PlayerTwoMineDeployedEvent -= PlayerTwoMineDeployed;
    }

    public void InitializeDisplayPanel(int wallInventory, int mineInventory)
    {
        UpdateScores();
        wallCount = wallInventory;
        mineCount = mineInventory;
        playerOneTankCount = 4;
        playerTwoTankCount = 4;
        topDisplayText.text = playerIndex == 0 ?
            "Welcome Team Purple Perfect !" :
            "Welcome Green Team of Awesomeness !";
        UpdateDisplayStatus();
    }

    /// <summary>
    /// Update scores on the heads up display.  If there was a win, show a temporary incrament on the screen while the confetti rains down/up
    /// </summary>
    /// <param name="incPlayerOneScore">Indicates that Player One just won, and needs a temporary incrament</param>
    /// <param name="incPlayerTwoScore">Indicates that Player Two just won, and needs a temporary incrament</param>
    private void UpdateScores(bool incPlayerOneScore=false, bool incPlayerTwoScore = false)
    {
        if (playerIndex == 0)  // Player 1
        {
            myWinCountText.text = (GameManager.playerOne.wins + (incPlayerOneScore ? 1 : 0)).ToString();
            enemyWinCountText.text = (GameManager.playerTwo.wins + (incPlayerTwoScore ? 1 : 0)).ToString();
        }
        else
        {
            myWinCountText.text = (GameManager.playerTwo.wins + (incPlayerTwoScore ? 1 : 0)).ToString();
            enemyWinCountText.text = (GameManager.playerOne.wins + (incPlayerOneScore ? 1 : 0)).ToString();
        }
    }

    private void PlayerOneTankHit()
    {
        if (playerIndex == 0)
        {
            MyTankWasHit();
            updateHUDHealth();
        }
    }

    private void PlayerTwoTankHit()
    {
        if (playerIndex == 1)
        {
            MyTankWasHit();
            updateHUDHealth();
        }
    }

    private void PlayerOneBaseHit()
    {
        if (playerIndex == 0)
        {
            MyBaseWasHit();
            updateHUDHealth();
        }
    }

    private void PlayerTwoBaseHit()
    {
        if (playerIndex == 1)
        {
            MyBaseWasHit();
            updateHUDHealth();
        }
    }
    private void PlayerOneAssetChanged()
    {
        if (playerIndex == 0)
            updateHUDHealth();
    }

    private void PlayerTwoAssetChanged()
    {
        if (playerIndex == 1) 
            updateHUDHealth();
    }

    private void updateHUDHealth()
    {
        if (playerIndex == 0)
            EventManager.StartPlayerOneUpdateHUDHealthEvent();
        else if (playerIndex == 1)
            EventManager.StartPlayerTwoUpdateHUDHealthEvent();
    }

    private void PlayerOneTankDefeated()
    {
        playerOneTankCount--;
        UpdateDisplayStatus();      
    }

    private void PlayerTwoTankDefeated()
    {
        playerTwoTankCount--;
        UpdateDisplayStatus();        
    }
    
    private void PlayerOneWallDeployed()
    {
        if (playerIndex == 0)
        {
            wallCount--;
            wallCountText.text = wallCount.ToString();
        }
    }

    private void PlayerTwoWallDeployed()
    {
        if (playerIndex == 1)
        {
            wallCount--;
            wallCountText.text = wallCount.ToString();
        }
    }

    private void PlayerOneMineDeployed()
    {
        if (playerIndex == 0)
        {
            mineCount--;
            mineCountText.text = mineCount.ToString();
        }
    }

    private void PlayerTwoMineDeployed()
    {
        if (playerIndex == 1)
        {
            mineCount--;
            mineCountText.text = mineCount.ToString();
        }
    }

    private void MyTankWasHit()
    {
        var myTankCount = playerIndex == 0 ? playerOneTankCount : playerTwoTankCount;

        switch (myTankCount)
        {
            case 1:
                myTankIcon4GoesHere.GetComponent<SpriteBlink>().blinkOnce();
                break;
            case 2:
                myTankIcon3GoesHere.GetComponent<SpriteBlink>().blinkOnce();
                break;
            case 3:
                myTankIcon2GoesHere.GetComponent<SpriteBlink>().blinkOnce();
                break;
            case 4:
                myTankIcon1GoesHere.GetComponent<SpriteBlink>().blinkOnce();                
                break;
            default:
                break;
        }
    }

    private void MyBaseWasHit()
    {        
        if (myTankIcon1GoesHere != null) 
            myTankIcon1GoesHere.GetComponent<SpriteBlink>().blinkOnce();
        if (myTankIcon2GoesHere != null) 
            myTankIcon2GoesHere.GetComponent<SpriteBlink>().blinkOnce();
        if (myTankIcon3GoesHere != null)
            myTankIcon3GoesHere.GetComponent<SpriteBlink>().blinkOnce();
        if (myTankIcon4GoesHere != null)
            myTankIcon4GoesHere.GetComponent<SpriteBlink>().blinkOnce();
    }


    private void UpdateDisplayStatus()
    {
        topDisplayText.text = playerIndex == 0 ?
            "Welcome Team Purple Perfect !" :
            "Welcome Green Team of Awesomeness !";
     
        var enemyTankCount = playerIndex == 0 ? playerTwoTankCount : playerOneTankCount;        

        enemyTankIcon1GoesHere.sprite = (enemyTankCount < 4) ? enemyTankExplodedIcon : enemyTankIcon;
        enemyTankIcon2GoesHere.sprite = (enemyTankCount < 3) ? enemyTankExplodedIcon : enemyTankIcon;
        enemyTankIcon3GoesHere.sprite = (enemyTankCount < 2) ? enemyTankExplodedIcon : enemyTankIcon;
        enemyTankIcon4GoesHere.sprite = (enemyTankCount < 1) ? enemyTankExplodedIcon : enemyTankIcon;

        var myTankCount = playerIndex == 0 ? playerOneTankCount : playerTwoTankCount;

        myTankIcon1GoesHere.sprite = (myTankCount < 4) ? myTankExplodedIcon : myTankIcon;
        myTankIcon2GoesHere.sprite = (myTankCount < 3) ? myTankExplodedIcon : myTankIcon;
        myTankIcon3GoesHere.sprite = (myTankCount < 2) ? myTankExplodedIcon : myTankIcon;
        myTankIcon4GoesHere.sprite = (myTankCount < 1) ? myTankExplodedIcon : myTankIcon;

        mineCountText.text = mineCount.ToString();
        wallCountText.text = wallCount.ToString();


        if (playerOneTankCount == 0)
            EventManager.StartCelebratePlayerOneDefeatedEvent();
        if (playerTwoTankCount == 0)
            EventManager.StartCelebratePlayerTwoDefeatedEvent();
    }



    private void ShowPlayerOneDefeated()
    {

        ShowDefeatedVictoryStatus(0);
    }

    private void ShowPlayerTwoDefeated()
    {
        ShowDefeatedVictoryStatus(1);
    }

    private void ShowDefeatedVictoryStatus(int playerIndexThatWasDefeated)
    {
        UpdateScores(playerIndexThatWasDefeated !=0, playerIndexThatWasDefeated != 1);
        if (playerIndex == playerIndexThatWasDefeated)
            topDisplayText.text = "Defeated !";
        else
            topDisplayText.text = "Victory !";
    }
}
