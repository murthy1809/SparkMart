using UnityEngine;

public class GoToCheckout : GAction {

    private Customer customer;

    void Start() {
        actionName = "GoToCheckout";
        
        // Preconditions: done shopping
        preconditions.Clear();
        preconditions.Add("doneShopping", 1);
        
        // Effects: in checkout queue
        effects.Clear();
        effects.Add("inCheckoutQueue", 1);
    }

    public override bool PrePerform() {
        customer = GetComponent<Customer>();
        if (customer == null) return false;

        // For testing without employees, skip the staffed check
        // if (!SparkWorld.Instance.GetWorld().HasState("StaffedCheckoutLane")) {
        //     return false;
        // }

        GameObject[] checkoutLanes = GameObject.FindGameObjectsWithTag("CheckoutLane");
        
        if (checkoutLanes.Length == 0) {
            Debug.LogWarning("No checkout lanes found!");
            return false;
        }

        // Just go to first checkout lane for now
        target = checkoutLanes[1];
        SparkWorld.Instance.GetQueue("customersInCheckoutQueue").AddResource(gameObject);

        return true;
    }

    public override bool PostPerform() {
        beliefs.ModifyState("inCheckoutQueue", 1);
        return true;
    }
}
