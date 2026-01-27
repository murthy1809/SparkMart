using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventory system for GOAP agents
/// </summary>
public class GInventory {

    public List<GameObject> items = new List<GameObject>();

    public void AddItem(GameObject item) {
        items.Add(item);
    }

    public GameObject FindItemWithTag(string tag) {
        foreach (GameObject item in items) {
            if (item == null) continue;
            if (item.CompareTag(tag)) {
                return item;
            }
        }
        return null;
    }

    public void RemoveItem(GameObject item) {
        items.Remove(item);
    }

    public void ClearItems() {
        items.Clear();
    }

    public int Count => items.Count;
}
