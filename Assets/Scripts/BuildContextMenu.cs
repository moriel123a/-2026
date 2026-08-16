using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildContextMenu : Singleton<BuildContextMenu>
{
    [Header("References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform panel;      // pivot MUST be top-left (0, 1)
    [SerializeField] private Transform rowContainer;    // holds a VerticalLayoutGroup
    [SerializeField] private BuildOptionRow rowPrefab;
    [SerializeField] private GameObject backgroundBlocker; // full-screen invisible Image + Button, closes menu on click
    [SerializeField] private ResourceDataSO resourceDatabase;

    private int pendingGridX, pendingGridY;
    private Vector3 pendingWorldPos;
    private readonly List<BuildOptionRow> spawnedRows = new List<BuildOptionRow>();

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        panel.gameObject.SetActive(false);
        backgroundBlocker.SetActive(false);
        backgroundBlocker.GetComponent<Button>().onClick.AddListener(Close);
    }

    // Called by GridManager when the player clicks a revealed, empty, unoccupied tile.
    public void Open(int gridX, int gridY, Vector3 worldPosition)
    {
        pendingGridX = gridX;
        pendingGridY = gridY;
        pendingWorldPos = worldPosition;

        BuildRows();

        panel.gameObject.SetActive(true);
        backgroundBlocker.SetActive(true);

        // Force an immediate layout pass so panel.rect reflects the real size of what
        // we just spawned, before we try to position/clamp it against the screen edges.
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);

        PositionAtMouse();
    }

    public void Close()
    {
        ClearRows();
        panel.gameObject.SetActive(false);
        backgroundBlocker.SetActive(false);
    }

    private void BuildRows()
    {
        ClearRows();

        foreach (var config in BuildManager.Instance.AllConfigs)
        {
            BuildOptionRow row = Instantiate(rowPrefab, rowContainer);
            bool canAfford = BuildManager.Instance.CanAfford(config);
            BuildingType type = config.type; // local copy for the closure below

            row.Setup(config, resourceDatabase, canAfford, () => OnBuildingSelected(type));
            spawnedRows.Add(row);
        }
    }

    private void ClearRows()
    {
        foreach (var row in spawnedRows)
        {
            if (row != null) Destroy(row.gameObject);
        }
        spawnedRows.Clear();
    }

    private void OnBuildingSelected(BuildingType type)
    {
        BuildManager.Instance.TryPlaceBuilding(type, pendingGridX, pendingGridY, pendingWorldPos);
        Close();
    }

    // Anchors the panel's top-left corner to the click position, like a Windows context
    // menu, then flips left/up if that would push any part of it off-screen.
    private void PositionAtMouse()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 localPos = screenPos / canvas.scaleFactor;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float menuWidth = panel.rect.width;
        float menuHeight = panel.rect.height;

        if (localPos.x + menuWidth > canvasRect.rect.width)
            localPos.x -= menuWidth; // flip left

        if (localPos.y - menuHeight < 0)
            localPos.y += menuHeight; // flip up (pivot is top-left, so the menu extends downward from localPos.y)

        panel.anchoredPosition = localPos;
    }
}