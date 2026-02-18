using UnityEngine;
using System.Collections.Generic;

public class RestockShelf : GAction
{

    private Employee employee;
    private Shelf targetShelf;
    private Transform restockPoint;

    void Start()
    {
        actionName = "RestockShelf";
        preconditions.Clear();
        effects.Clear();
        preconditions["hasStock"] = 1;
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

        // Pick the lowest stock shelf that isn't already being restocked
        targetShelf = null;
        foreach (Shelf shelf in needsRestock)
        {
            ShelfDestinationManager destManager = shelf.GetComponent<ShelfDestinationManager>();
            if (destManager != null && destManager.IsRestockOccupied()) continue;

            if (targetShelf == null || shelf.CurrentStock < targetShelf.CurrentStock)
            {
                targetShelf = shelf;
            }
        }

        if (targetShelf == null) return false;

        // Claim the restock point
        ShelfDestinationManager manager = targetShelf.GetComponent<ShelfDestinationManager>();
        if (manager != null && !manager.ClaimRestockPoint(gameObject))
        {
            return false;
        }

        target = targetShelf.gameObject;
        duration = employee.GetAdjustedDuration(targetShelf.GetRestockDuration());

        // Use the dedicated RestockPoint so we don't block customer destinations
        if (manager != null)
        {
            restockPoint = manager.GetRestockPoint();
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
        // Release restock point
        if (targetShelf != null)
        {
            ShelfDestinationManager manager = targetShelf.GetComponent<ShelfDestinationManager>();
            if (manager != null)
            {
                manager.ReleaseRestockPoint();
            }
        }

        if (targetShelf != null && employee.HasStock)
        {
            int delivered = employee.DeliverStock();
            targetShelf.AddItems(delivered);
        }

        beliefs.ModifyState("shelvesRestocked", 1);
        beliefs.RemoveState("shelvesRestocked");
        beliefs.RemoveState("hasStock");

        targetShelf = null;
        restockPoint = null;
        return true;
    }

    public Transform GetRestockDestination()
    {
        return restockPoint;
    }
}