using System;
using UnityEngine;

public enum BuildingType
{
    Wall,
    Catapult
}

[Serializable]
public struct BuildingConfig
{
    public BuildingType type;
    public string displayName;
    public Sprite icon;
    public Building prefab;
    public BuildingCostSO cost;
}