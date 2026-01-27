using UnityEngine;

public class OperateCheckout : GAction {

    private Employee employee;
    private Customer currentCustomer;
    public float baseProcessTime = 3f;
    public float timePerItem = 0.5f;

    public override bool PrePerform() {
        employee = GetComponent<Employee>();
        if (employee == null) return false;

        if (!employee.IsAtCheckoutLane) {
            return false;
        }

        ResourceQueue customerQueue = SparkWorld.Instance.GetQueue("customersInCheckoutQueue");
        if (customerQueue == null || customerQueue.Count == 0) {
            return false;
        }

        GameObject customerObj = customerQueue.RemoveResource();
        if (customerObj == null) {
            return false;
        }

        currentCustomer = customerObj.GetComponent<Customer>();
        if (currentCustomer == null) {
            return false;
        }

        target = customerObj;

        float processTime = baseProcessTime + (currentCustomer.ItemsInCart * timePerItem);
        duration = employee.GetAdjustedDuration(processTime);

        return true;
    }

    public override bool PostPerform() {
        if (currentCustomer != null) {
            currentCustomer.CompleteCheckout();
        }

        beliefs.ModifyState("operatingCheckout", 1);
        beliefs.RemoveState("operatingCheckout");

        currentCustomer = null;
        return true;
    }
}
