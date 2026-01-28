using UnityEngine;

public class GoToShelf : GAction {

    private Customer customer;
    private Shelf targetShelf;

    void Start() {
        // Set up action chain
        actionName = "GoToShelf";
        
        // Effects: produces atShelf
        effects.Clear();
        effects.Add("atShelf", 1);
        
        // No preconditions
        preconditions.Clear();
    }

    public override bool PrePerform() {
        customer = GetComponent<Customer>();
        if (customer == null) return false;

        if (customer.Persona.requiresCart && !customer.HasCart) {
            return false;
        }

        ShelfType? nextItem = customer.GetNextShoppingItem();
        if (nextItem == null) {
            beliefs.ModifyState("doneShopping", 1);
            return false;
        }

        targetShelf = SparkWorld.Instance.GetShelfByType(nextItem.Value);

        if (targetShelf == null) {
            targetShelf = SparkWorld.Instance.GetRandomStockedShelf();
        }

        if (targetShelf == null) {
            customer.OnItemOutOfStock();
            return false;
        }

        target = targetShelf.gameObject;
        customer.SetCurrentShelf(targetShelf);

        return true;
    }

    public override bool PostPerform() {
        beliefs.ModifyState("atShelf", 1);
        return true;
    }
}
