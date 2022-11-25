using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MilkShake;

public class Damagable : MonoBehaviour
{
    [SerializeField] private int MaxHealth = 100;
    [SerializeField] private ShakePreset shakePresetHit;
    [SerializeField] private ShakePreset shakePresetDestroyed;

    public int health;
    private AssetController assetController;

    public int Health
    {
        get { return health; }
        set 
        { 
            health = value;
            OnHealthChanged?.Invoke((float)Health / MaxHealth);
        }
    }

    public UnityEvent OnDead;
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnHit, OnHeal;

    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
        assetController = gameObject.GetComponentInChildren<AssetController>();
    }

    public void RestoreAllHealth()
    {
        Health = MaxHealth;
    }

    internal void Hit(int damagePoints)
    {
        Health -= damagePoints;
        if (Health <= 0) 
        { 
            OnDead?.Invoke();
            if (amITheEngagedAsset)
                Shaker.ShakeOne(assetController.playerIndex, shakePresetDestroyed);
        }
        else 
        { 
            // Land Mines have bigger damage - show a bigger shake
            OnHit?.Invoke();            
            if (amITheEngagedAsset)
                Shaker.ShakeOne(assetController.playerIndex,
                    damagePoints >= 10 ? shakePresetDestroyed : shakePresetHit);
        }
    }

    private bool amITheEngagedAsset => (assetController != null) && assetController.isThisTheEngagedAsset;    

    public void SelfDistruct()
    {
        // Could do more with this in the future
        // perhaps neighboring damagables take a hit or two ?
        Health = 0;
        OnDead?.Invoke();
        if (amITheEngagedAsset)
            Shaker.ShakeOne(assetController.playerIndex, shakePresetDestroyed);
    }

    public void Heal(int healthBoost)
    {
        Health += healthBoost;
        Health = Mathf.Clamp(Health, 0, MaxHealth);
        OnHeal?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
