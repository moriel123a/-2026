using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCostRow : MonoBehaviour
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TMP_Text resourceCost;

    public void Setup(Sprite icon, int cost)
    {
        resourceIcon.sprite = icon;
        resourceCost.text = cost.ToString();
    }

}
