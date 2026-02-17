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

        GameObject[] checkoutLanes = GameObject.FindGameObjectsWithTag("CheckoutLane");
        
        if (checkoutLanes.Length == 0) {
            Debug.LogWarning("No checkout lanes found!");
            return false;
        }

        // Find closest checkout lane
        GameObject closestLane = checkoutLanes[0];
        float closestDist = Vector3.Distance(transform.position, closestLane.transform.position);
        
        foreach (GameObject lane in checkoutLanes) {
            float dist = Vector3.Distance(transform.position, lane.transform.position);
            if (dist < closestDist) {
                closestDist = dist;
                closestLane = lane;
            }
        }

        target = closestLane;
        SparkWorld.Instance.GetQueue("customersInCheckoutQueue").AddResource(gameObject);

        return true;
    }

    public override bool PostPerform() {
        beliefs.ModifyState("inCheckoutQueue", 1);
        return true;
    }
}
