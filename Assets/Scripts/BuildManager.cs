using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Building Configs")]
    [Tooltip("Add one entry per BuildingType, each with its prefab and a BuildingCostSO asset.")]
    public List<BuildingConfig> buildingConfigs;

    private Dictionary<BuildingType, BuildingConfig> configLookup;

    public List<Building> ActiveBuildings { get; } = new List<Building>();

    private BuildingType? selectedType;

    void Awake()
    {
        Instance = this;

        configLookup = new Dictionary<BuildingType, BuildingConfig>();
        foreach (var config in buildingConfigs)
        {
            configLookup[config.type] = config;
        }
    }

    // Hook these up to UI buttons.
    public void SelectWall() => selectedType = BuildingType.Wall;
    public void SelectCatapult() => selectedType = BuildingType.Catapult;
    public void CancelSelection() => selectedType = null;

    // Called by GridManager when the player clicks a revealed, empty, unoccupied tile.
    public bool TryPlaceBuilding(int gridX, int gridY, Vector3 worldPosition)
    {
        if (selectedType == null) return false;

        if (!configLookup.TryGetValue(selectedType.Value, out BuildingConfig config))
        {
            Debug.LogWarning($"No BuildingConfig registered for {selectedType.Value}.");
            return false;
        }

        if (!ResourceManager.Instance.TrySpend(config.cost.woodCost, config.cost.stoneCost))
        {
            Debug.Log("Not enough resources.");
            return false;
        }

        Building building = Instantiate(config.prefab, worldPosition, Quaternion.identity);
        building.SetGridPosition(gridX, gridY);
        ActiveBuildings.Add(building);

        GridManager.Instance.SetOccupant(gridX, gridY, building);

        selectedType = null; // one placement per selection - drop this line to allow multi-placing
        return true;
    }

    public void NotifyBuildingDestroyed(Building building)
    {
        ActiveBuildings.Remove(building);
        GridManager.Instance.ClearOccupant(building.GridX, building.GridY);
    }
}