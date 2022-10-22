using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{
    [SerializeField] private int MaxHealth = 100;
    public int health;

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
    }

    internal void Hit(int damagePoints)
    {
        Health -= damagePoints;
        if (Health <= 0) { OnDead?.Invoke(); }
        else { OnHit?.Invoke(); }
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
