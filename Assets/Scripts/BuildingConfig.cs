using System;

public enum BuildingType
{
    Wall,
    Catapult
}

[Serializable]
public struct BuildingConfig
{
    public BuildingType type;
    public Building prefab;
    public BuildingCostSO cost;
}
