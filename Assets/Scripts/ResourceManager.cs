using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public int Wood { get; private set; }
    public int Stone { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void AddWood(int amount) => Wood += amount;
    public void AddStone(int amount) => Stone += amount;

    public bool TrySpend(int woodCost, int stoneCost)
    {
        if (Wood < woodCost || Stone < stoneCost) return false;

        Wood -= woodCost;
        Stone -= stoneCost;
        return true;
    }
}