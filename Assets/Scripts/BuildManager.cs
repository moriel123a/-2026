using System.Collections.Generic;
using UnityEngine;

public class BuildManager : Singleton<BuildManager>
{
    [Header("Building Configs")]
    [Tooltip("Add one entry per BuildingType, each with its prefab and a BuildingCostSO asset.")]
    public List<BuildingConfig> buildingConfigs;

    private Dictionary<BuildingType, BuildingConfig> configLookup;

    public List<Building> ActiveBuildings { get; } = new List<Building>();

    private BuildingType? selectedType;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return; // this instance is a duplicate about to be destroyed

        configLookup = new Dictionary<BuildingType, BuildingConfig>();
        foreach (var config in buildingConfigs)
        {
            configLookup[config.type] = config;
        }
    }

    // Hook these up to UI buttons if you still want a "select then click a tile" flow elsewhere.
    public void SelectWall() => selectedType = BuildingType.Wall;
    public void SelectCatapult() => selectedType = BuildingType.Catapult;
    public void CancelSelection() => selectedType = null;

    public IEnumerable<BuildingConfig> AllConfigs => configLookup.Values;

    public bool CanAfford(BuildingConfig config)
    {
        return ResourceManager.Instance.CanAfford(config.cost.costs);
    }

    // Used by the "select a building, then click a tile" flow.
    public bool TryPlaceBuilding(int gridX, int gridY, Vector3 worldPosition)
    {
        if (selectedType == null) return false;
        return TryPlaceBuilding(selectedType.Value, gridX, gridY, worldPosition);
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

        selectedType = null; // clear in case a "select" flow was also in progress
        return true;
    }

    public void NotifyBuildingDestroyed(Building building)
    {
        ActiveBuildings.Remove(building);
        GridManager.Instance.ClearOccupant(building.GridX, building.GridY);
    }
}