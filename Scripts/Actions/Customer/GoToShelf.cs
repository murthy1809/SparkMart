using UnityEngine;

public class GoToShelf : GAction
{

    private Customer customer;
    private Shelf targetShelf;
    private ShelfDestinationManager destinationManager;
    private Transform claimedSpot;

    void Start()
    {
        preconditions.Clear();
        effects.Clear();
        effects["atShelf"] = 1;
    }

    public override bool PrePerform()
    {
        customer = GetComponent<Customer>();
        if (customer == null) return false;

        if (customer.Persona.requiresCart && !customer.HasCart)
        {
            return false;
        }

        ShelfType? nextItem = customer.GetNextShoppingItem();
        if (nextItem == null)
        {
            beliefs.ModifyState("doneShopping", 1);
            return false;
        }

        targetShelf = SparkWorld.Instance.GetShelfByType(nextItem.Value);

        if (targetShelf == null)
        {
            targetShelf = SparkWorld.Instance.GetRandomStockedShelf();
        }

        if (targetShelf == null)
        {
            customer.OnItemOutOfStock();
            return false;
        }

        // --- Destination point selection ---
        destinationManager = targetShelf.GetComponent<ShelfDestinationManager>();

        if (destinationManager != null && destinationManager.HasAvailableSpot())
        {
            // Get nearest unoccupied spot
            claimedSpot = destinationManager.GetNearestAvailableSpot(transform.position);

            if (claimedSpot != null)
            {
                destinationManager.ClaimSpot(claimedSpot, gameObject);
            }
        }

        target = targetShelf.gameObject;
        customer.SetCurrentShelf(targetShelf);

        return true;
    }

    public override bool PostPerform()
    {
        beliefs.ModifyState("atShelf", 1);
        return true;
    }

    /// <summary>
    /// Called by GAgent when navigating — override destination if we claimed a spot.
    /// Returns the claimed spot position, or null to use default behavior.
    /// </summary>
    public Transform GetClaimedDestination()
    {
        return claimedSpot;
    }

    /// <summary>
    /// Call this to release the claimed spot (called from PickUpItem.PostPerform).
    /// </summary>
    public void ReleaseClaimedSpot()
    {
        if (destinationManager != null && claimedSpot != null)
        {
            destinationManager.ReleaseSpot(claimedSpot);
            claimedSpot = null;
        }
    }
}