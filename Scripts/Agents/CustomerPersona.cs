using UnityEngine;

public enum PersonaType {
    QuickShopper,
    Browser,
    BulkBuyer,
    Troublemaker
}

[CreateAssetMenu(fileName = "NewPersona", menuName = "SparkMart/Customer Persona", order = 2)]
public class CustomerPersona : ScriptableObject {

    [Header("Identity")]
    public string personaName = "Quick Shopper";
    public PersonaType personaType = PersonaType.QuickShopper;
    public Color debugColor = Color.blue;

    [Header("Spawn")]
    [Range(0, 100)] public int spawnWeight = 25;

    [Header("Shopping Preferences")]
    [Range(0, 100)] public int weightGeneral = 30;
    [Range(0, 100)] public int weightProduce = 25;
    [Range(0, 100)] public int weightFrozen = 15;
    [Range(0, 100)] public int weightElectronics = 15;
    [Range(0, 100)] public int weightClothing = 15;

    public ShelfType GetWeightedRandomShelfType()
    {
        int total = weightGeneral + weightProduce + weightFrozen + weightElectronics + weightClothing;
        if (total <= 0) return ShelfType.General;

        int roll = Random.Range(0, total);
        int cumulative = 0;

        cumulative += weightGeneral;
        if (roll < cumulative) return ShelfType.General;

        cumulative += weightProduce;
        if (roll < cumulative) return ShelfType.Produce;

        cumulative += weightFrozen;
        if (roll < cumulative) return ShelfType.Frozen;

        cumulative += weightElectronics;
        if (roll < cumulative) return ShelfType.Electronics;

        return ShelfType.Clothing;
    }

    [Header("Shopping")]
    [Range(0, 20)] public int minShoppingListSize = 1;
    [Range(0, 20)] public int maxShoppingListSize = 3;
    public bool requiresCart = false;
    public bool prefersSelfCheckout = true;
    public bool browsesBehavior = false;

    [Header("Patience")]
    [Range(10f, 300f)] public float basePatience = 60f;
    [Range(0.5f, 3f)] public float impatienceMultiplier = 1f;
    [Range(0, 100)] public int startingSatisfaction = 70;

    [Header("Troublemaker")]
    [Range(0f, 1f)] public float fightChance = 0f;
    [Range(0f, 1f)] public float stealChance = 0f;
    [Range(0f, 1f)] public float spillChance = 0.01f;

    [Header("Movement")]
    [Range(1f, 5f)] public float moveSpeed = 3.5f;

    public int GenerateShoppingListSize() {
        return Random.Range(minShoppingListSize, maxShoppingListSize + 1);
    }

    public float GetEffectivePatience() {
        return basePatience / impatienceMultiplier;
    }

    public bool ShouldAttemptFight() {
        return Random.value < fightChance;
    }

    public bool ShouldAttemptSteal() {
        return Random.value < stealChance;
    }

    public bool ShouldCreateSpill() {
        return Random.value < spillChance;
    }
}
