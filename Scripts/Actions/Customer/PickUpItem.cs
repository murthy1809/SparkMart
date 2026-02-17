using UnityEngine;

public class PickUpItem : GAction
{

    private Customer customer;

    void Start()
    {
        preconditions.Clear();
        effects.Clear();
        preconditions["atShelf"] = 1;
        effects["doneShopping"] = 1;
    }

    public override bool PrePerform()
    {
        customer = GetComponent<Customer>();
        if (customer == null) return false;

        Shelf shelf = customer.GetCurrentShelf();
        if (shelf == null) return false;

        if (!shelf.HasStock())
        {
            customer.OnItemOutOfStock();
            beliefs.RemoveState("atShelf");
            ReleaseShelfSpot();
            return false;
        }

        target = shelf.gameObject;
        duration = shelf.GetBrowseTime();

        return true;
    }

    public override bool PostPerform()
    {
        Shelf shelf = customer.GetCurrentShelf();

        if (shelf != null && shelf.HasStock())
        {
            int taken = shelf.TakeItems(1);

            if (taken > 0)
            {
                customer.CollectItem(shelf, taken);
                beliefs.ModifyState("hasItems", 1);
            }
        }

        beliefs.RemoveState("atShelf");

        // Release the destination spot so other customers can use it
        ReleaseShelfSpot();

        if (customer.GetRemainingItems() <= 0)
        {
            beliefs.ModifyState("doneShopping", 1);
            beliefs.ModifyState("readyToCheckout", 1);
        }

        return true;
    }

    /// <summary>
    /// Finds the GoToShelf action on this agent and releases its claimed spot.
    /// </summary>
    private void ReleaseShelfSpot()
    {
        GoToShelf goToShelf = GetComponent<GoToShelf>();
        if (goToShelf != null)
        {
            goToShelf.ReleaseClaimedSpot();
        }
    }
}