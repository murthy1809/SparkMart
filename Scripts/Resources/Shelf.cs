using UnityEngine;

public class Shelf : MonoBehaviour {

    [Header("Configuration")]
    public ShelfData shelfData;

    [Header("Current State")]
    [SerializeField] private int currentStock;

    public int CurrentStock => currentStock;
    public float StockPercentage => shelfData != null ? (float)currentStock / shelfData.maxStock : 0f;

    public System.Action<Shelf> OnStockChanged;

    void Awake() {
        if (shelfData == null) {
            Debug.LogError($"Shelf {gameObject.name} has no ShelfData assigned!");
            return;
        }
        currentStock = shelfData.initialStock;
    }

    public bool NeedsRestock() {
        if (shelfData == null) return false;
        return currentStock <= shelfData.restockThreshold;
    }

    public bool HasStock() {
        return currentStock > 0;
    }

    public bool IsFull() {
        if (shelfData == null) return true;
        return currentStock >= shelfData.maxStock;
    }

    public int TakeItems(int amount = 1) {
        if (shelfData == null || currentStock <= 0) return 0;

        int taken = Mathf.Min(amount, currentStock);
        currentStock -= taken;
        OnStockChanged?.Invoke(this);
        return taken;
    }

    public int AddItems(int amount = -1) {
        if (shelfData == null) return 0;

        int previousStock = currentStock;

        if (amount < 0) {
            currentStock = shelfData.maxStock;
        } else {
            currentStock = Mathf.Min(currentStock + amount, shelfData.maxStock);
        }

        int added = currentStock - previousStock;
        if (added > 0) {
            OnStockChanged?.Invoke(this);
        }
        return added;
    }

    public void Restock() {
        AddItems(-1);
    }

    public float GetProfit(int itemCount) {
        if (shelfData == null) return 0f;
        return itemCount * shelfData.profitPerItem;
    }

    public float GetBrowseTime() {
        if (shelfData == null) return 3f;
        return shelfData.browseTime;
    }

    public float GetRestockDuration() {
        if (shelfData == null) return 10f;
        return shelfData.restockDuration;
    }

    void OnDrawGizmos() {
        Gizmos.color = HasStock() ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.up, Vector3.one);
    }
}
