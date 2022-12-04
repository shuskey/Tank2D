using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class HUDHealthController : MonoBehaviour
{
    [SerializeField] GameObject baseCamp;

    private void Awake()
    {
        EventManager.PlayerOneUpdateHUDHealthEvent += UpdatePlayerOneHealth;
        EventManager.PlayerTwoUpdateHUDHealthEvent += UpdatePlayerTwoHealth;
    }

    private void OnDisable()
    {
        EventManager.PlayerOneUpdateHUDHealthEvent -= UpdatePlayerOneHealth;
        EventManager.PlayerTwoUpdateHUDHealthEvent -= UpdatePlayerTwoHealth;
    }

    private void UpdatePlayerOneHealth()
    {
        if (baseCamp.GetComponent<BaseCampController>().playerIndexThatOwnsThisBaseCamp == 0)
            UpdateHealth();
    }

    private void UpdatePlayerTwoHealth()
    {
        if (baseCamp.GetComponent<BaseCampController>().playerIndexThatOwnsThisBaseCamp == 1)
            UpdateHealth();
    }

    // Current Selected Asset Health Percent Amount         
    private void UpdateHealth()
    {
        float health = baseCamp.GetComponent<BaseCampController>().getHealthOfCurrentAsset();
        var gradiantImage = GetComponent<Image>();
        Color currentColor = gradiantImage.color;
        currentColor.a = 1 - health / 100;
        gradiantImage.color = currentColor;        
    }
}
