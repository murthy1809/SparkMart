using UnityEngine;

public class GoToCheckoutLane : GAction {

    private Employee employee;

    public override bool PrePerform() {
        employee = GetComponent<Employee>();
        if (employee == null) return false;

        if (!employee.assignedToCheckout) {
            return false;
        }

        if (employee.IsAtCheckoutLane) {
            beliefs.ModifyState("atCheckoutLane", 1);
            return false;
        }

        if (!SparkWorld.Instance.GetWorld().HasState("FreeCheckoutLane")) {
            return false;
        }

        GameObject lane = SparkWorld.Instance.GetQueue("checkoutLanes").RemoveResource();

        if (lane == null) {
            return false;
        }

        target = lane;
        return true;
    }

    public override bool PostPerform() {
        employee.AssignToCheckoutLane(target);

        CheckoutLane laneComponent = target.GetComponent<CheckoutLane>();
        if (laneComponent != null) {
            laneComponent.AssignEmployee(employee);
        }

        beliefs.ModifyState("atCheckoutLane", 1);
        return true;
    }
}
