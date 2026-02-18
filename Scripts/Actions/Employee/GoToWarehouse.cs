using UnityEngine;

public class GoToWarehouse : GAction {

    private Employee employee;

    void Start() {
        actionName = "GoToWarehouse";
        preconditions.Clear();
        effects.Clear();
        effects["hasStock"] = 1;
    }

    public override bool PrePerform() {
        employee = GetComponent<Employee>();
        if (employee == null) return false;

        if (!employee.assignedToRestock) return false;

        // Only go to warehouse if not already carrying stock
        if (employee.HasStock) return false;

        // Only go if there are shelves that need restocking
        if (SparkWorld.Instance.GetShelvesNeedingRestock().Count == 0) return false;

        GameObject warehouse = GameObject.FindWithTag("Warehouse");
        if (warehouse == null) {
            Debug.LogWarning("No Warehouse found! Add a GameObject with tag 'Warehouse'.");
            return false;
        }

        target = warehouse;
        duration = 2f;

        return true;
    }

    public override bool PostPerform() {
        employee.PickUpStock(employee.maxCarryCapacity);
        beliefs.ModifyState("hasStock", 1);
        return true;
    }
}
