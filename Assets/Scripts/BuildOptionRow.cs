using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildOptionRow : MonoBehaviour
{
    [Header("Left")]
    [SerializeField] private Image buildingIcon;

    [Header("Center")]
    [SerializeField] private TMP_Text nameText;

    [Header("Right - Resource Costs")]
    [SerializeField] private Image woodIcon;
    [SerializeField] private TMP_Text woodCostText;
    [SerializeField] private Image stoneIcon;
    [SerializeField] private TMP_Text stoneCostText;

    [Header("State")]
    [SerializeField] private Button button;
    [SerializeField] private CanvasGroup canvasGroup; // dims icon/name/cost together when unaffordable

    public void Setup(BuildingConfig config, ResourceDataSO resourceDatabase, bool canAfford, Action onClick)
    {
        buildingIcon.sprite = config.icon;
        nameText.text = config.displayName;

        woodIcon.sprite = resourceDatabase.GetIcon(ResourceType.Wood);
        woodCostText.text = config.cost.GetCost(ResourceType.Wood).ToString();

        stoneIcon.sprite = resourceDatabase.GetIcon(ResourceType.Stone);
        stoneCostText.text = config.cost.GetCost(ResourceType.Stone).ToString();

        button.interactable = canAfford;
        canvasGroup.alpha = canAfford ? 1f : 0.4f;

        button.onClick.RemoveAllListeners();
        if (canAfford)
        {
            button.onClick.AddListener(() => onClick());
        }
    }
}