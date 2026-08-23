using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A base class for buildings that can be placed by the player.
/// </summary>
public class Building : MonoBehaviour
{
    public int maxHealth = 30;
    protected int currentHealth;

    public int GridX { get; private set; }
    public int GridY { get; private set; }
    public event Action<int> OnHealthChanged;
    public event Action OnBuildingDestroyed;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetGridPosition(int x, int y)
    {
        GridX = x;
        GridY = y;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            OnDestroyed();
        }
    }

    protected virtual void OnDestroyed()
    {
        OnBuildingDestroyed?.Invoke();
        BuildManager.Instance.NotifyBuildingDestroyed(this);
        Destroy(gameObject);
    }
}