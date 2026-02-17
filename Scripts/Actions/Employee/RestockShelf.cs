using UnityEngine;
using System.Collections.Generic;

public class RestockShelf : GAction
{

    private Employee employee;
    private Shelf targetShelf;
    private Transform restockPoint;

    void Start()
    {
        preconditions.Clear();
        effects.Clear();
        effects["shelvesRestocked"] = 1;
    }

    public override bool PrePerform()
    {
        employee = GetComponent<Employee>();
        if (employee == null) return false;

        if (!employee.assignedToRestock)
        {
            return false;
        }

        if (employee.IsAtCheckoutLane)
        {
            return false;
        }

        List<Shelf> needsRestock = SparkWorld.Instance.GetShelvesNeedingRestock();

        if (needsRestock.Count == 0)
        {
            return false;
        }

        // Pick the shelf with lowest stock (most urgent)
        targetShelf = needsRestock[0];
        foreach (Shelf shelf in needsRestock)
        {
            if (shelf.CurrentStock < targetShelf.CurrentStock)
            {
                targetShelf = shelf;
            }
        }

        target = targetShelf.gameObject;
        duration = employee.GetAdjustedDuration(targetShelf.GetRestockDuration());

        // Use the dedicated RestockPoint so we don't block customer destinations
        ShelfDestinationManager destManager = targetShelf.GetComponent<ShelfDestinationManager>();
        if (destManager != null)
        {
            restockPoint = destManager.GetRestockPoint();
        }

        // Fallback: look for a RestockPoint child directly
        if (restockPoint == null)
        {
            restockPoint = targetShelf.transform.Find("RestockPoint");
        }

        return true;
    }

    public override bool PostPerform()
    {
        if (targetShelf != null)
        {
            employee.RestockShelf(targetShelf);
        }

        beliefs.ModifyState("shelvesRestocked", 1);
        beliefs.RemoveState("shelvesRestocked");

        targetShelf = null;
        restockPoint = null;
        return true;
    }

    /// <summary>
    /// Returns the restock-specific destination point.
    /// GAgent will use this instead of the default "Destination" child.
    /// </summary>
    public Transform GetRestockDestination()
    {
        return restockPoint;
    }
}