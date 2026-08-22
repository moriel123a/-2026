using System;

/// <summary>
/// An enum with all resource types.
/// </summary>
public enum ResourceType
{
    Wood,
    Stone
}


/// <summary>
/// A struct to represent a single resource cost of a building.
/// </summary>
[Serializable]
public struct ResourceCost
{
    public ResourceType type;
    public int amount;
}