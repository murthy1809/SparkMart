using UnityEngine;

public class GoToCheckout : GAction {

    private Customer customer;

    public override bool PrePerform() {
        customer = GetComponent<Customer>();
        if (customer == null) return false;

        if (!SparkWorld.Instance.GetWorld().HasState("StaffedCheckoutLane")) {
            return false;
        }

        GameObject[] checkoutLanes = GameObject.FindGameObjectsWithTag("CheckoutLane");
        GameObject targetLane = null;

        foreach (GameObject lane in checkoutLanes) {
            CheckoutLane checkoutComponent = lane.GetComponent<CheckoutLane>();
            if (checkoutComponent != null && checkoutComponent.IsStaffed) {
                targetLane = lane;
                break;
            }
        }

        if (targetLane == null) {
            return false;
        }

        target = targetLane;
        SparkWorld.Instance.GetQueue("customersInCheckoutQueue").AddResource(gameObject);

        return true;
    }

    public override bool PostPerform() {
        beliefs.ModifyState("inCheckoutQueue", 1);
        return true;
    }
}
