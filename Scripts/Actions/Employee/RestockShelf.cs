using UnityEngine;
using System.Collections.Generic;

public class RestockShelf : GAction {

    private Employee employee;
    private Shelf targetShelf;

    public override bool PrePerform() {
        employee = GetComponent<Employee>();
        if (employee == null) return false;

        if (!employee.assignedToRestock) {
            return false;
        }

        if (employee.IsAtCheckoutLane) {
            return false;
        }

        List<Shelf> needsRestock = SparkWorld.Instance.GetShelvesNeedingRestock();

        if (needsRestock.Count == 0) {
            return false;
        }

        targetShelf = needsRestock[0];
        foreach (Shelf shelf in needsRestock) {
            if (shelf.CurrentStock < targetShelf.CurrentStock) {
                targetShelf = shelf;
            }
        }

        target = targetShelf.gameObject;
        duration = employee.GetAdjustedDuration(targetShelf.GetRestockDuration());

        return true;
    }

    public override bool PostPerform() {
        if (targetShelf != null) {
            employee.RestockShelf(targetShelf);
        }

        beliefs.ModifyState("shelvesRestocked", 1);
        beliefs.RemoveState("shelvesRestocked");

        targetShelf = null;
        return true;
    }
}
