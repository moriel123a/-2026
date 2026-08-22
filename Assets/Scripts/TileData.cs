/// <summary>
/// Enum with all the stuff that can be revealed under a hidden tile.
/// </summary>
public enum TileType
{
    Empty,
    Tree,
    Stone,
    Enemy
}

/// <summary>
/// Class with information about a single tile.
/// </summary>
[System.Serializable]
public class TileData
{
    public int x;
    public int y;
    public TileType type;
    public bool isRevealed;
    public bool isHarvested;
    public Building occupant;

    public TileData(int x, int y, TileType type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.isRevealed = false;
        this.isHarvested = false;
        this.occupant = null;
    }
}
