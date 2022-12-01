using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class HUDHealthController : MonoBehaviour
{
    [SerializeField] GameObject baseCamp;

    // Current Selected Asset Health Percent Amount         
    public void UpdateHealth()
    {
        float health = baseCamp.GetComponent<BaseCampController>().getHealthOfCurrentAsset();
        var gradiantImage = GetComponent<Image>();
        Color currentColor = gradiantImage.color;
        currentColor.a = 1 - health / 100;
        gradiantImage.color = currentColor;        
    }
}
