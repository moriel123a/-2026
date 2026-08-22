using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A single row inside the building context menu.
/// Represents a single building option.
/// </summary>
public class BuildOptionRow : MonoBehaviour
{
    [Header("Left")]
    [SerializeField] private Image buildingIcon;

    [Header("Center")]
    [SerializeField] private TMP_Text nameText;

    [Header("Right - Resource Costs")]
    [SerializeField] private GameObject resourceCostContainer;
    [SerializeField] private GameObject resourceCostPrefab;

    [Header("State")]
    [SerializeField] private Button button;
    [SerializeField] private CanvasGroup canvasGroup; // dims icon/name/cost together when unaffordable

    public void Setup(BuildingConfig config, ResourceDataSO resourceDatabase, bool canAfford, Action onClick)
    {
        buildingIcon.sprite = config.icon;
        nameText.text = config.displayName;

        button.interactable = canAfford;
        canvasGroup.alpha = canAfford ? 1f : 0.4f;

        button.onClick.RemoveAllListeners();
        if (canAfford)
        {
            button.onClick.AddListener(() => onClick());
        }
    }
}