using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceDatabase", menuName = "TowerDefense/Resource Database")]
public class ResourceDataSO : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public ResourceType type;
        public Sprite icon;
    }

    [SerializeField] private List<Entry> entries;

    private Dictionary<ResourceType, Sprite> lookup;

    public Sprite GetIcon(ResourceType type)
    {
        if (lookup == null) BuildLookup();

        lookup.TryGetValue(type, out Sprite icon);
        return icon;
    }

    private void BuildLookup()
    {
        lookup = new Dictionary<ResourceType, Sprite>();
        foreach (var entry in entries)
        {
            lookup[entry.type] = entry.icon;
        }
    }
}