using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object with the build cost of a single building.
/// Used inside the build config.
/// </summary>
[CreateAssetMenu(fileName = "BuildingCost", menuName = "TowerDefense/Building Cost")]
public class BuildingCostSO : ScriptableObject
{
    public List<ResourceCost> costs;

    public int GetCost(ResourceType type)
    {
        foreach (var cost in costs)
        {
            if (cost.type == type) return cost.amount;
        }
        return 0;
    }
}