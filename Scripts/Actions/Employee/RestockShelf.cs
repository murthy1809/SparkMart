using UnityEngine;
using System.Collections.Generic;

public class RestockShelf : GAction
{

    private Employee employee;
    private Shelf targetShelf;

    void Start()
    {
        actionName = "RestockShelf";

        preconditions.Clear();

        effects.Clear();
        effects.Add("shelvesRestocked", 1);
    }

    public override bool PrePerform()
    {
        employee = GetComponent<Employee>();
        if (employee == null)
        {
            Debug.Log("[RestockShelf] No Employee component!");
            return false;
        }

        if (!employee.assignedToRestock)
        {
            Debug.Log("[RestockShelf] Employee not assigned to restock");
            return false;
        }

        if (employee.IsAtCheckoutLane)
        {
            Debug.Log("[RestockShelf] Employee is at checkout lane, can't restock");
            return false;
        }

        List<Shelf> needsRestock = SparkWorld.Instance.GetShelvesNeedingRestock();
        Debug.Log($"[RestockShelf] Shelves needing restock: {needsRestock.Count}");

        if (needsRestock.Count == 0)
        {
            Debug.Log("[RestockShelf] No shelves need restocking");
            return false;
        }

        // Find closest shelf that needs restock
        targetShelf = needsRestock[0];
        float closestDist = Vector3.Distance(transform.position, targetShelf.transform.position);

        foreach (Shelf shelf in needsRestock)
        {
            float dist = Vector3.Distance(transform.position, shelf.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                targetShelf = shelf;
            }
        }

        target = targetShelf.gameObject;
        duration = employee.GetAdjustedDuration(targetShelf.GetRestockDuration());

        Debug.Log($"[RestockShelf] Going to restock {targetShelf.name}");

        return true;
    }

    public override bool PostPerform()
    {
        if (targetShelf != null)
        {
            employee.RestockShelf(targetShelf);
        }

        targetShelf = null;
        return true;
    }
}