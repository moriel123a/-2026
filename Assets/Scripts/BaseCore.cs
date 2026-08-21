using System;
using UnityEngine;

public class BaseCore : Singleton<BaseCore>
{
    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    public event Action<int> OnHealthChanged;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return; // this instance is a duplicate about to be destroyed

        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            GameOver();
        }
    }

    public bool GameOver()
    {
        Debug.Log("Base destroyed. Game over.");
        return true;
    }
}