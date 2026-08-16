using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [Header("Grid Settings")]
    public int width = 11;
    public int height = 11;
    public float tileSpacing = 1.1f;

    [Header("Tile Content Weights (relative chance)")]
    public float emptyWeight = 40f;
    public float treeWeight = 25f;
    public float stoneWeight = 25f;
    public float enemyWeight = 10f;

    [Header("Reveal Settings")]
    public int minClusterSize = 3;
    public int maxClusterSize = 6; // inclusive

    [Header("Resource Yields")]
    public int woodPerTree = 2;
    public int stonePerStone = 2;

    [Header("Prefabs")]
    public TileView tilePrefab;
    public BaseCore basePrefab;

    private TileData[,] grid;
    private TileView[,] views;
    private int centerX, centerY;

    void Start()
    {
        centerX = width / 2;
        centerY = height / 2;
        GenerateGrid();
        SpawnBase();
    }

    void GenerateGrid()
    {
        grid = new TileData[width, height];
        views = new TileView[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isCenter = x == centerX && y == centerY;
                TileType type = isCenter ? TileType.Empty : RollTileType();
                grid[x, y] = new TileData(x, y, type);

                TileView view = Instantiate(tilePrefab, GridToWorld(x, y), Quaternion.identity, transform);
                view.Init(x, y, this);
                views[x, y] = view;

                // Base tile starts revealed so the player always sees home from the start.
                if (isCenter)
                {
                    grid[x, y].isRevealed = true;
                    view.ShowRevealed(TileType.Empty);
                }
            }
        }
    }

    void SpawnBase()
    {
        BaseCore baseInstance = Instantiate(basePrefab, GridToWorld(centerX, centerY), Quaternion.identity);
    }

    public Vector3 GridToWorld(int x, int y)
    {
        // Offset so that (centerX, centerY) - the base tile - lands exactly on this GameObject's position.
        float offsetX = (x - centerX) * tileSpacing;
        float offsetY = (y - centerY) * tileSpacing;
        return transform.position + new Vector3(offsetX, offsetY, 0f);
    }

    TileType RollTileType()
    {
        float total = emptyWeight + treeWeight + stoneWeight + enemyWeight;
        float roll = Random.Range(0f, total);

        if (roll < emptyWeight) return TileType.Empty;
        roll -= emptyWeight;
        if (roll < treeWeight) return TileType.Tree;
        roll -= treeWeight;
        if (roll < stoneWeight) return TileType.Stone;
        return TileType.Enemy;
    }

    public void OnTileClicked(int x, int y)
    {
        if (!grid[x, y].isRevealed)
        {
            RevealCluster(x, y);
        }
        else
        {
            OnRevealedTileClicked(x, y);
        }
    }

    void RevealCluster(int startX, int startY)
    {
        int targetSize = Random.Range(minClusterSize, maxClusterSize + 1);
        List<Vector2Int> cluster = GetRandomConnectedCluster(startX, startY, targetSize);

        foreach (var pos in cluster)
        {
            RevealTile(pos.x, pos.y);
        }
    }

    // Grows a connected blob starting at (startX, startY) by repeatedly picking a
    // random tile from the frontier (unrevealed neighbors of the selection so far).
    List<Vector2Int> GetRandomConnectedCluster(int startX, int startY, int targetSize)
    {
        List<Vector2Int> selected = new List<Vector2Int>();
        HashSet<Vector2Int> selectedSet = new HashSet<Vector2Int>();
        List<Vector2Int> frontier = new List<Vector2Int>();

        Vector2Int start = new Vector2Int(startX, startY);
        selected.Add(start);
        selectedSet.Add(start);
        AddNeighborsToFrontier(start, selectedSet, frontier);

        while (selected.Count < targetSize && frontier.Count > 0)
        {
            int idx = Random.Range(0, frontier.Count);
            Vector2Int next = frontier[idx];
            frontier.RemoveAt(idx);

            if (selectedSet.Contains(next)) continue;
            if (grid[next.x, next.y].isRevealed) continue;

            selected.Add(next);
            selectedSet.Add(next);
            AddNeighborsToFrontier(next, selectedSet, frontier);
        }

        return selected;
    }

    void AddNeighborsToFrontier(Vector2Int tile, HashSet<Vector2Int> selectedSet, List<Vector2Int> frontier)
    {
        Vector2Int[] dirs =
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1)
        };

        foreach (var d in dirs)
        {
            Vector2Int n = tile + d;
            if (n.x < 0 || n.x >= width || n.y < 0 || n.y >= height) continue;
            if (selectedSet.Contains(n)) continue;
            if (grid[n.x, n.y].isRevealed) continue;

            frontier.Add(n);
        }
    }

    void RevealTile(int x, int y)
    {
        TileData tile = grid[x, y];
        tile.isRevealed = true;
        views[x, y].ShowRevealed(tile.type);

        // Enemies aren't tile decoration - revealing one spawns a mobile unit that
        // immediately heads for the base. The tile itself becomes plain ground.
        if (tile.type == TileType.Enemy)
        {
            EnemyManager.Instance.SpawnEnemy(GridToWorld(x, y));
        }
    }

    void OnRevealedTileClicked(int x, int y)
    {
        TileData tile = grid[x, y];

        bool isHarvestableResource = (tile.type == TileType.Tree || tile.type == TileType.Stone) && !tile.isHarvested;
        if (isHarvestableResource)
        {
            HarvestTile(x, y);
            return;
        }

        // Buildable: plain ground, a tile an enemy already left, or a harvested resource tile - as long as nothing is on it.
        bool isBuildable = tile.occupant == null &&
            (tile.type == TileType.Empty || tile.type == TileType.Enemy || tile.isHarvested);

        if (isBuildable)
        {
            BuildContextMenu.Instance.Open(x, y, GridToWorld(x, y));
        }
    }

    void HarvestTile(int x, int y)
    {
        TileData tile = grid[x, y];
        tile.isHarvested = true;

        if (tile.type == TileType.Tree)
            ResourceManager.Instance.AddResource(ResourceType.Wood, woodPerTree);
        else if (tile.type == TileType.Stone)
            ResourceManager.Instance.AddResource(ResourceType.Stone, stonePerStone);

        views[x, y].ShowHarvested();
    }

    public void SetOccupant(int x, int y, Building building)
    {
        grid[x, y].occupant = building;
    }

    public void ClearOccupant(int x, int y)
    {
        grid[x, y].occupant = null;
    }
}