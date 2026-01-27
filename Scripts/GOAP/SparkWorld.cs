using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Resource queue for managing shared resources in SparkMart
/// </summary>
public class ResourceQueue {

    public Queue<GameObject> queue = new Queue<GameObject>();
    public string tag;
    public string modState;

    public ResourceQueue(string tag, string modState, WorldStates worldStates) {
        this.tag = tag;
        this.modState = modState;

        if (!string.IsNullOrEmpty(tag)) {
            GameObject[] resources = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject r in resources) {
                queue.Enqueue(r);
            }
        }

        if (!string.IsNullOrEmpty(modState) && worldStates != null) {
            worldStates.ModifyState(modState, queue.Count);
        }
    }

    public void AddResource(GameObject resource) {
        queue.Enqueue(resource);
    }

    public GameObject RemoveResource() {
        if (queue.Count == 0) return null;
        return queue.Dequeue();
    }

    public void RemoveResource(GameObject resource) {
        queue = new Queue<GameObject>(queue.Where(r => r != resource));
    }

    public int Count => queue.Count;
}

/// <summary>
/// Singleton world manager for SparkMart simulation
/// </summary>
public sealed class SparkWorld : MonoBehaviour {

    private static SparkWorld _instance;
    public static SparkWorld Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<SparkWorld>();
                if (_instance == null) {
                    GameObject go = new GameObject("SparkWorld");
                    _instance = go.AddComponent<SparkWorld>();
                }
            }
            return _instance;
        }
    }

    private WorldStates world;
    private Dictionary<string, ResourceQueue> resources = new Dictionary<string, ResourceQueue>();

    [Header("Time Settings")]
    [Range(0.1f, 10f)]
    public float timeScale = 1.0f;

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeWorld();
    }

    void InitializeWorld() {
        world = new WorldStates();

        // Shopping Carts
        resources.Add("carts", new ResourceQueue("Cart", "FreeCart", world));

        // Checkout Lanes
        resources.Add("checkoutLanes", new ResourceQueue("CheckoutLane", "FreeCheckoutLane", world));

        // Self-Checkout
        resources.Add("selfCheckouts", new ResourceQueue("SelfCheckout", "FreeSelfCheckout", world));

        // Restrooms
        resources.Add("restrooms", new ResourceQueue("Restroom", "FreeRestroom", world));

        // Break Room
        resources.Add("breakRoom", new ResourceQueue("BreakRoom", "FreeBreakRoom", world));

        // Shelves by type
        resources.Add("generalShelves", new ResourceQueue("GeneralShelf", "FreeGeneralShelf", world));
        resources.Add("produceShelves", new ResourceQueue("ProduceShelf", "FreeProduceShelf", world));
        resources.Add("frozenShelves", new ResourceQueue("FrozenShelf", "FreeFrozenShelf", world));
        resources.Add("electronicsShelves", new ResourceQueue("ElectronicsShelf", "FreeElectronicsShelf", world));
        resources.Add("clothingShelves", new ResourceQueue("ClothingShelf", "FreeClothingShelf", world));

        // Dynamic queues (start empty)
        resources.Add("spills", new ResourceQueue("", "", world));
        resources.Add("fights", new ResourceQueue("", "", world));
        resources.Add("injuredCustomers", new ResourceQueue("", "", world));

        // Agent queues
        resources.Add("employees", new ResourceQueue("", "", world));
        resources.Add("janitors", new ResourceQueue("", "", world));
        resources.Add("securityGuards", new ResourceQueue("", "", world));
        resources.Add("customersWaitingForHelp", new ResourceQueue("", "", world));
        resources.Add("customersInCheckoutQueue", new ResourceQueue("", "", world));

        Debug.Log("SparkWorld initialized!");
    }

    void Update() {
        Time.timeScale = timeScale;
    }

    public WorldStates GetWorld() {
        return world;
    }

    public ResourceQueue GetQueue(string queueName) {
        if (resources.ContainsKey(queueName)) {
            return resources[queueName];
        }
        Debug.LogWarning($"ResourceQueue '{queueName}' not found!");
        return null;
    }

    public void AddQueue(string queueName, string tag = "", string modState = "") {
        if (!resources.ContainsKey(queueName)) {
            resources.Add(queueName, new ResourceQueue(tag, modState, world));
        }
    }

    public List<Shelf> GetShelvesNeedingRestock() {
        List<Shelf> needsRestock = new List<Shelf>();
        Shelf[] allShelves = FindObjectsOfType<Shelf>();

        foreach (Shelf shelf in allShelves) {
            if (shelf.NeedsRestock()) {
                needsRestock.Add(shelf);
            }
        }
        return needsRestock;
    }

    public Shelf GetRandomStockedShelf() {
        Shelf[] allShelves = FindObjectsOfType<Shelf>();
        List<Shelf> stockedShelves = new List<Shelf>();

        foreach (Shelf shelf in allShelves) {
            if (shelf.HasStock()) {
                stockedShelves.Add(shelf);
            }
        }

        if (stockedShelves.Count == 0) return null;
        return stockedShelves[Random.Range(0, stockedShelves.Count)];
    }

    public Shelf GetShelfByType(ShelfType type) {
        Shelf[] allShelves = FindObjectsOfType<Shelf>();
        List<Shelf> matchingShelves = new List<Shelf>();

        foreach (Shelf shelf in allShelves) {
            if (shelf.shelfData != null && shelf.shelfData.shelfType == type && shelf.HasStock()) {
                matchingShelves.Add(shelf);
            }
        }

        if (matchingShelves.Count == 0) return null;
        return matchingShelves[Random.Range(0, matchingShelves.Count)];
    }
}
