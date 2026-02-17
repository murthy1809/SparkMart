using UnityEngine;

public class OperateCheckout : GAction {

    private Employee employee;
    private Customer currentCustomer;
    public float baseProcessTime = 3f;
    public float timePerItem = 0.5f;

    void Start() {
        actionName = "OperateCheckout";
        
        // Preconditions: must be at checkout lane
        preconditions.Clear();
        preconditions.Add("atCheckoutLane", 1);
        
        // Effects: operating checkout (continuous goal)
        effects.Clear();
        effects.Add("operatingCheckout", 1);
    }

    public override bool PrePerform() {
        employee = GetComponent<Employee>();
        if (employee == null) return false;

        if (!employee.IsAtCheckoutLane) {
            return false;
        }

        ResourceQueue customerQueue = SparkWorld.Instance.GetQueue("customersInCheckoutQueue");
        if (customerQueue == null || customerQueue.Count == 0) {
            // No customers - just wait at checkout
            target = employee.AssignedCheckoutLane;
            duration = 1f; // Check again in 1 second
            return true;
        }

        GameObject customerObj = customerQueue.RemoveResource();
        if (customerObj == null) {
            target = employee.AssignedCheckoutLane;
            duration = 1f;
            return true;
        }

        currentCustomer = customerObj.GetComponent<Customer>();
        if (currentCustomer == null) {
            target = employee.AssignedCheckoutLane;
            duration = 1f;
            return true;
        }

        target = employee.AssignedCheckoutLane;

        float processTime = baseProcessTime + (currentCustomer.ItemsInCart * timePerItem);
        duration = employee.GetAdjustedDuration(processTime);

        return true;
    }

    public override bool PostPerform() {
        if (currentCustomer != null) {
            currentCustomer.CompleteCheckout();
        }

        currentCustomer = null;
        return true;
    }
}
