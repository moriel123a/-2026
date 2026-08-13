using UnityEngine;

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

    private int x, y;
    private GridManager grid;

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
    }

    public void ShowHarvested()
    {
        if (icon != null)
            icon.enabled = false;
    }
}