using System;
using UnityEngine;

/// <summary>
/// Enum for every placeable building.
/// </summary>
public enum BuildingType
{
    Wall,
    Catapult
}

/// <summary>
/// A struct for building information that shows up in the context menu.
/// Used inside the build manager.
/// </summary>
[Serializable]
public struct BuildingConfig
{
    public BuildingType type;
    public string displayName;
    public Sprite icon;
    public Building prefab;
    public BuildingCostSO cost;
}