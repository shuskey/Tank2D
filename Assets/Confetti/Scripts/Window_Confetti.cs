using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using static EventManager;

public class Window_Confetti : MonoBehaviour {

    [SerializeField] private Transform pfConfetti;
    [SerializeField] private Color[] winColorArray;
    [SerializeField] private Color[] loseColorArray;

    private List<Confetti> confettiList;
    private bool showerOn = false;
    private bool showerDirectionDownForTheWin = true;
    private float spawnTimer;
    private float showerTimer;
    private const float SPAWN_TIMER_MAX = 0.013f;
    private const float SHOWER_TIMER_MAX = 3.0f;
    private Canvas myCanvas;
   
    private void Awake()
    {
        myCanvas = GetComponentInParent<Canvas>();

        confettiList = new List<Confetti>();

        // Subscribe to Events
        EventManager.PlayerOneDefeatedEvent += ShowPlayerOneDefeated;
        EventManager.PlayerTwoDefeatedEvent += ShowPlayerTwoDefeated;
    }

    private void OnDestroy()
    {
        EventManager.PlayerOneDefeatedEvent -= ShowPlayerOneDefeated;
        EventManager.PlayerTwoDefeatedEvent -= ShowPlayerTwoDefeated;
    }

    private void ShowPlayerOneDefeated()
    {
      //  Debug.Log($"ShowerOn: {showerOn}  ShowPlayer ONE Defeated called: DiaplyIndex {myCanvas.targetDisplay}");
        ShowDefeatedVictoryStatus(0);
    }

    private void ShowPlayerTwoDefeated()
    {
      //  Debug.Log($"ShowerOn: {showerOn}  ShowPlayer TWO Defeated called: DisplayIndex {myCanvas.targetDisplay}");
        ShowDefeatedVictoryStatus(1);
    }

    private void ShowDefeatedVictoryStatus(int playerIndexThatWasDefeated)
    {

        if (myCanvas.worldCamera.targetDisplay == playerIndexThatWasDefeated)
        {
            StartShower(isVictory: false);
        }
        else
        {
            StartShower();
        }
    }

    private void StartShower(bool isVictory = true)
    {
        if (isVictory)
        {
            showerDirectionDownForTheWin = true;
        }
        else
        {
            showerDirectionDownForTheWin = false;
        }

        showerOn = true;
        showerTimer = SHOWER_TIMER_MAX;
    }

    private void Update() {
 
        foreach (Confetti confetti in new List<Confetti>(confettiList))
        {
            if (confetti.Update(showerDirectionDownForTheWin))
            {
                confettiList.Remove(confetti);
            }
        }

        if (showerOn)
        {
            showerTimer -= Time.deltaTime;

            if (showerTimer > 0f)
            {
                spawnTimer -= Time.deltaTime;
                if (spawnTimer <= 0f)
                {
                    spawnTimer += SPAWN_TIMER_MAX;
                    int spawnAmount = Random.Range(1, 4);
                    for (int i = 0; i < spawnAmount; i++)
                    {
                        SpawnConfetti();
                    }
                }
            }
            else
            {
                showerOn = false;
            }
 
        }
    }

    private void SpawnConfetti() {
        float width = transform.GetComponent<RectTransform>().rect.width;
        float height = transform.GetComponent<RectTransform>().rect.height;
        Vector2 anchoredPosition = new Vector2(Random.Range(-width / 2f, width / 2f), (showerDirectionDownForTheWin ? 1f : -1f) * height / 2f);
        Color color = GetWinOrLoseColor(showerDirectionDownForTheWin);
        Confetti confetti = new Confetti(pfConfetti,
            transform.parent.transform,
            anchoredPosition,
            color,
            height / 2f,
            showerDirectionDownForTheWin);
        confettiList.Add(confetti);
        
        Color GetWinOrLoseColor(bool isWin)
        {
            if (isWin)
                return winColorArray[Random.Range(0, winColorArray.Length)];
            return loseColorArray[Random.Range(0, loseColorArray.Length)];
        }
    }

    private class Confetti {

        private Transform transform;
        private RectTransform rectTransform;
        private Vector2 anchoredPosition;
        private Vector3 euler;
        private float eulerSpeed;
        private Vector2 moveAmount;
        private float minimumY;

        public Confetti(Transform prefab, 
            Transform container, 
            Vector2 anchoredPosition, 
            Color color, 
            float minimumY, 
            bool showerDirectionDown) 
        {
            this.anchoredPosition = anchoredPosition;
            this.minimumY = (showerDirectionDown ? -1f : 1f) * minimumY;
            transform = Instantiate(prefab, container);
            rectTransform = transform.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;

            rectTransform.sizeDelta *= Random.Range(.8f, 1.2f);

            euler = new Vector3(0, 0, Random.Range(0, 360f));
            eulerSpeed = Random.Range(100f, 200f);
            eulerSpeed *= Random.Range(0, 2) == 0 ? 1f : -1f;
            transform.localEulerAngles = euler;

            moveAmount = new Vector2(0, (showerDirectionDown ? -1f : 1f) * Random.Range(150f, 600f));

            transform.GetComponent<Image>().color = color;
        }

        public bool Update(bool showerDirectionDown) {
            anchoredPosition += moveAmount * Time.deltaTime;
            rectTransform.anchoredPosition = anchoredPosition;

            euler.z += eulerSpeed * Time.deltaTime;
            transform.localEulerAngles = euler;

            if ((showerDirectionDown && (anchoredPosition.y < minimumY))
                ||
                (!showerDirectionDown && (anchoredPosition.y > minimumY))
                ) {
                Destroy(transform.gameObject);
                return true;
            } else {
                return false;
            }
        }
    }
}

