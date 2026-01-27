using UnityEngine;

public enum ShelfType {
    General,
    Produce,
    Frozen,
    Electronics,
    Clothing
}

[CreateAssetMenu(fileName = "NewShelfData", menuName = "SparkMart/Shelf Data", order = 1)]
public class ShelfData : ScriptableObject {

    [Header("Identity")]
    public string shelfName = "General Shelf";
    public ShelfType shelfType = ShelfType.General;

    [Header("Stock")]
    [Range(0, 100)] public int minStock = 0;
    [Range(1, 100)] public int maxStock = 100;
    [Range(0, 100)] public int restockThreshold = 20;
    [Range(0, 100)] public int initialStock = 100;

    [Header("Economy")]
    public float profitPerItem = 8f;
    [Range(0f, 1f)] public float theftRisk = 0.1f;

    [Header("Timing")]
    public float restockDuration = 10f;
    public float browseTime = 3f;

    public string GetQueueName() {
        return shelfType switch {
            ShelfType.General => "generalShelves",
            ShelfType.Produce => "produceShelves",
            ShelfType.Frozen => "frozenShelves",
            ShelfType.Electronics => "electronicsShelves",
            ShelfType.Clothing => "clothingShelves",
            _ => "generalShelves"
        };
    }

    public string GetTag() {
        return shelfType switch {
            ShelfType.General => "GeneralShelf",
            ShelfType.Produce => "ProduceShelf",
            ShelfType.Frozen => "FrozenShelf",
            ShelfType.Electronics => "ElectronicsShelf",
            ShelfType.Clothing => "ClothingShelf",
            _ => "GeneralShelf"
        };
    }
}
