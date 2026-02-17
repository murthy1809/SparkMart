using UnityEngine;

public class PickUpItem : GAction {

    private Customer customer;

    void Start() {
        actionName = "PickUpItem";
        
        // Preconditions: needs to be at shelf
        preconditions.Clear();
        preconditions.Add("atShelf", 1);
        
        // Effects: produces doneShopping
        effects.Clear();
        effects.Add("doneShopping", 1);
    }

    public override bool PrePerform() {
        customer = GetComponent<Customer>();
        if (customer == null) return false;

        Shelf shelf = customer.GetCurrentShelf();
        if (shelf == null) return false;

        if (!shelf.HasStock()) {
            customer.OnItemOutOfStock();
            beliefs.RemoveState("atShelf");
            return false;
        }

        target = shelf.gameObject;
        
        // Duration = browse time × number of remaining items
        int remainingItems = customer.GetRemainingItems();
        duration = shelf.GetBrowseTime() * remainingItems;

        return true;
    }

    public override bool PostPerform() {
        Shelf shelf = customer.GetCurrentShelf();

        // Pick up ALL remaining items
        int remainingItems = customer.GetRemainingItems();
        
        if (shelf != null && shelf.HasStock()) {
            for (int i = 0; i < remainingItems; i++) {
                int taken = shelf.TakeItems(1);
                if (taken > 0) {
                    customer.CollectItem(shelf, taken);
                }
            }
            beliefs.ModifyState("hasItems", 1);
        }

        beliefs.RemoveState("atShelf");
        
        // Now we're definitely done shopping
        beliefs.ModifyState("doneShopping", 1);
        beliefs.ModifyState("readyToCheckout", 1);

        return true;
    }
}
