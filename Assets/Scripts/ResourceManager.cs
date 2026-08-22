using System;
using System.Collections.Generic;

/// <summary>
/// A class managing the resources that the player has.
/// Use to get and spend resources.
/// </summary>
public class ResourceManager : Singleton<ResourceManager>
{
    private readonly Dictionary<ResourceType, int> amounts = new Dictionary<ResourceType, int>();

    // Fired whenever a resource amount changes - passes the type and its new total.
    public event Action<ResourceType, int> OnResourceChanged;

    public int GetAmount(ResourceType type)
    {
        amounts.TryGetValue(type, out int amount);
        return amount;
    }

    public void AddResource(ResourceType type, int amount)
    {
        int newAmount = GetAmount(type) + amount;
        amounts[type] = newAmount;
        OnResourceChanged?.Invoke(type, newAmount);
    }

    /// <summary>
    /// Checks if the player can afford a given building cost.
    /// </summary>
    /// <param name="costs">The cost of the building.</param>
    /// <returns></returns>
    public bool CanAfford(IEnumerable<ResourceCost> costs)
    {
        foreach (var cost in costs)
        {
            if (GetAmount(cost.type) < cost.amount) return false;
        }
        return true;
    }

    public bool TrySpend(IEnumerable<ResourceCost> costs)
    {
        if (!CanAfford(costs)) return false;

        foreach (var cost in costs)
        {
            int newAmount = GetAmount(cost.type) - cost.amount;
            amounts[cost.type] = newAmount;
            OnResourceChanged?.Invoke(cost.type, newAmount);
        }

        return true;
    }
}