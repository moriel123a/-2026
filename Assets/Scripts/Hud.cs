using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// The script responsible for HUD functions.
/// </summary>
public class HUD : MonoBehaviour // TODO: switch wood and stone text to be use icons and be instantiated in runtime using a loop in case of more resources.
{
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text stoneText;
    [SerializeField] private TMP_Text baseHealthText;

    void Start()
    {
        // ResourceManager already exists in the scene at Start, so this is safe immediately.
        ResourceManager.Instance.OnResourceChanged += OnResourceChanged;

        UpdateWoodText(ResourceManager.Instance.GetAmount(ResourceType.Wood));
        UpdateStoneText(ResourceManager.Instance.GetAmount(ResourceType.Stone));

        // BaseCore is Instantiated at runtime by GridManager.Start(), so it may not
        // exist yet here - wait for it instead of assuming it's already spawned.
        StartCoroutine(SubscribeToBaseCoreWhenReady());
    }

    IEnumerator SubscribeToBaseCoreWhenReady()
    {
        yield return new WaitUntil(() => GridManager.Instance.playerBase != null);

        BaseCore playerBase = GridManager.Instance.playerBase;
        playerBase.OnHealthChanged += UpdateHealthText;
        UpdateHealthText(playerBase.CurrentHealth);
    }

    void OnDestroy()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
        }

        if (GridManager.Instance.playerBase != null)
        {
            GridManager.Instance.playerBase.OnHealthChanged -= UpdateHealthText;
        }
    }

    private void OnResourceChanged(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                UpdateWoodText(amount);
                break;
            case ResourceType.Stone:
                UpdateStoneText(amount);
                break;
        }
    }

    private void UpdateWoodText(int amount) => woodText.text = $"Wood: {amount}";
    private void UpdateStoneText(int amount) => stoneText.text = $"Stone: {amount}";
    private void UpdateHealthText(int health) => baseHealthText.text = $"Health: {health}";
}