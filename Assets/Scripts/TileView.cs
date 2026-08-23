using TMPro;
using UnityEngine;

/// <summary>
/// The class for a tile prefab that is visible in the game.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TileView : MonoBehaviour, IClickable
{
    [Header("Visuals")]
    public SpriteRenderer background;
    public SpriteRenderer icon;

    public Sprite hiddenSprite;
    public Sprite revealedSprite;

    public Sprite treeIcon;
    public Sprite stoneIcon;

    public TMP_Text safeNumber;
    public TMP_Text dangerNumber;

    private int x, y;
    private GridManager grid;

    private void Start()
    {
        ShowNumbers(false);
    }
    public void Init(int x, int y, GridManager grid)
    {
        this.x = x;
        this.y = y;
        this.grid = grid;

        if (background != null && hiddenSprite != null)
            background.sprite = hiddenSprite;

        if (icon != null)
            icon.enabled = false;
    }

    public void HandleClick()
    {
        grid.OnTileClicked(x, y);
    }

    /// <summary>
    /// Happens when the tile is revealed to show what's beneath it. Called from the grid manager.
    /// </summary>
    /// <param name="type">The object under the revealed tile.</param>
    public void ShowRevealed(TileType type)
    {
        if (background != null && revealedSprite != null)
            background.sprite = revealedSprite;

        if (icon == null) return;

        switch (type)
        {
            case TileType.Tree:
                icon.enabled = true;
                icon.sprite = treeIcon;
                break;
            case TileType.Stone:
                icon.enabled = true;
                icon.sprite = stoneIcon;
                break;
            case TileType.Empty:
            case TileType.Enemy: // the enemy has already left as a mobile unit by the time this is called
            default:
                icon.enabled = false;
                break;
        }

        ShowNumbers(true);
    }

    /// <summary>
    /// Called when the resource in the tile is harvested to remove it.
    /// </summary>
    public void ShowHarvested()
    {
        if (icon != null)
            icon.enabled = false;
    }

    /// <summary>
    /// Set the safe numers to enabled/disabled used at start and when revealed
    /// </summary>
    /// <param name="show">Whether to numbers or not</param>
    private void ShowNumbers(bool show) 
    {
        dangerNumber.enabled = show;
        safeNumber.enabled = show;
    }

    public void SetSafeNumber(int number)
    {
        safeNumber.text = number.ToString();
    }

    public void SetDangerNumber(int number)
    {
        dangerNumber.text = number.ToString();
    }
}