using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to contain the build actions, and building information.
/// Use this class to build and destroy buildings.
/// </summary>
public class BuildManager : Singleton<BuildManager>
{
    [Header("Building Configs")]
    [Tooltip("Add one entry per BuildingType, each with its prefab and a BuildingCostSO asset.")]
    public List<BuildingConfig> buildingConfigs;

    private Dictionary<BuildingType, BuildingConfig> configLookup; // Information about all buildable buildings.

    public List<Building> ActiveBuildings { get; } = new List<Building>();

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return; // this instance is a duplicate about to be destroyed

        configLookup = new Dictionary<BuildingType, BuildingConfig>(); // Converts the list to a dictionary for fasater lookup.
        foreach (var config in buildingConfigs)
        {
            configLookup[config.type] = config;
        }
    }

    public IEnumerable<BuildingConfig> AllConfigs => configLookup.Values;

    public bool CanAfford(BuildingConfig config)
    {
        return ResourceManager.Instance.CanAfford(config.cost.costs);
    }

    // Used by the context menu, which already knows exactly which building was chosen.
    public bool TryPlaceBuilding(BuildingType type, int gridX, int gridY, Vector3 worldPosition)
    {
        if (!configLookup.TryGetValue(type, out BuildingConfig config))
        {
            Debug.LogWarning($"No BuildingConfig registered for {type}.");
            return false;
        }

        if (!ResourceManager.Instance.TrySpend(config.cost.costs))
        {
            Debug.Log("Not enough resources.");
            return false;
        }

        Building building = Instantiate(config.prefab, worldPosition, Quaternion.identity);
        building.SetGridPosition(gridX, gridY);
        ActiveBuildings.Add(building);

        GridManager.Instance.SetOccupant(gridX, gridY, building);

        return true;
    }

    // Called when an enemy destroys a building to remove it from the boards.
    public void NotifyBuildingDestroyed(Building building)
    {
        ActiveBuildings.Remove(building);
        GridManager.Instance.ClearOccupant(building.GridX, building.GridY);
    }
}