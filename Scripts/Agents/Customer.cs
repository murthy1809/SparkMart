using System.Collections.Generic;
using UnityEngine;

public class Customer : GAgent {

    [Header("Customer Configuration")]
    public CustomerPersona persona;

    [Header("Shopping State")]
    [SerializeField] private int shoppingListSize;
    [SerializeField] private int itemsCollected;
    [SerializeField] private int itemsInCart;
    [SerializeField] private float currentSatisfaction;
    [SerializeField] private float currentPatience;
    [SerializeField] private float totalProfit;

    private List<ShelfType> shoppingList = new List<ShelfType>();
    private int currentShoppingIndex = 0;
    private GameObject cart;
    private Shelf currentShelf;

    public int ItemsCollected => itemsCollected;
    public int ItemsInCart => itemsInCart;
    public float Satisfaction => currentSatisfaction;
    public float TotalProfit => totalProfit;
    public bool HasCart => cart != null;
    public CustomerPersona Persona => persona;

    public System.Action<Customer> OnCheckoutComplete;
    public System.Action<Customer> OnLeftStore;

    public override void Start() {
        base.Start();

        if (persona == null) {
            Debug.LogError("Customer has no persona assigned!");
            return;
        }

        InitializeCustomer();
        SetupGoals();
    }

    void InitializeCustomer() {
        shoppingListSize = persona.GenerateShoppingListSize();
        GenerateShoppingList();

        currentSatisfaction = persona.startingSatisfaction;
        currentPatience = persona.basePatience;
        itemsCollected = 0;
        itemsInCart = 0;
        totalProfit = 0f;

        if (currentAction != null && currentAction.agent != null) {
            currentAction.agent.speed = persona.moveSpeed;
        }
    }

    void GenerateShoppingList() {
        shoppingList.Clear();

        for (int i = 0; i < shoppingListSize; i++) {
            ShelfType type = (ShelfType)Random.Range(0, System.Enum.GetValues(typeof(ShelfType)).Length);
            shoppingList.Add(type);
        }

        if (persona.browsesBehavior && shoppingListSize == 0) {
            int browseItems = Random.Range(1, 4);
            for (int i = 0; i < browseItems; i++) {
                ShelfType type = (ShelfType)Random.Range(0, System.Enum.GetValues(typeof(ShelfType)).Length);
                shoppingList.Add(type);
            }
        }
    }

    void SetupGoals() {
        if (persona.requiresCart) {
            SubGoal getCartGoal = new SubGoal("hasCart", 1, true);
            goals.Add(getCartGoal, 5);
        }

        SubGoal shopGoal = new SubGoal("doneShopping", 1, true);
        goals.Add(shopGoal, 4);

        SubGoal checkoutGoal = new SubGoal("hasCheckedOut", 1, true);
        goals.Add(checkoutGoal, 3);

        SubGoal homeGoal = new SubGoal("leftStore", 1, true);
        goals.Add(homeGoal, 1);
    }

    void Update() {
        if (currentPatience > 0) {
            currentPatience -= Time.deltaTime * persona.impatienceMultiplier;

            if (currentPatience <= 0) {
                OnPatienceExpired();
            }
        }
    }

    void OnPatienceExpired() {
        ModifySatisfaction(-20);
        beliefs.ModifyState("isFrustrated", 1);
    }

    public ShelfType? GetNextShoppingItem() {
        if (currentShoppingIndex >= shoppingList.Count) {
            return null;
        }
        return shoppingList[currentShoppingIndex];
    }

    public void CollectItem(Shelf shelf, int count = 1) {
        itemsCollected += count;
        itemsInCart += count;
        currentShoppingIndex++;
        totalProfit += shelf.GetProfit(count);

        if (currentShoppingIndex >= shoppingList.Count) {
            beliefs.ModifyState("doneShopping", 1);
            beliefs.ModifyState("readyToCheckout", 1);
        }

        currentShelf = null;
    }

    public void OnItemOutOfStock() {
        ModifySatisfaction(-5);
        beliefs.ModifyState("isFrustrated", 1);
        currentShoppingIndex++;

        if (currentShoppingIndex >= shoppingList.Count) {
            beliefs.ModifyState("doneShopping", 1);
            beliefs.ModifyState("readyToCheckout", 1);
        }
    }

    public void SetCurrentShelf(Shelf shelf) {
        currentShelf = shelf;
    }

    public Shelf GetCurrentShelf() {
        return currentShelf;
    }

    public void AssignCart(GameObject cartObj) {
        cart = cartObj;
        beliefs.ModifyState("hasCart", 1);
    }

    public GameObject ReturnCart() {
        GameObject returnedCart = cart;
        cart = null;
        beliefs.RemoveState("hasCart");
        return returnedCart;
    }

    public void ModifySatisfaction(float amount) {
        currentSatisfaction = Mathf.Clamp(currentSatisfaction + amount, 0f, 100f);
    }

    public void CompleteCheckout() {
        beliefs.ModifyState("hasCheckedOut", 1);

        if (currentPatience > persona.basePatience * 0.5f) {
            ModifySatisfaction(5);
        }

        OnCheckoutComplete?.Invoke(this);
    }

    public void LeaveStore() {
        if (cart != null) {
            SparkWorld.Instance.GetQueue("carts").AddResource(cart);
            SparkWorld.Instance.GetWorld().ModifyState("FreeCart", 1);
            cart = null;
        }

        OnLeftStore?.Invoke(this);
        Destroy(gameObject, 0.5f);
    }

    public int GetRemainingItems() {
        return Mathf.Max(0, shoppingList.Count - currentShoppingIndex);
    }

    void OnDrawGizmosSelected() {
        if (persona != null) {
            Gizmos.color = persona.debugColor;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
