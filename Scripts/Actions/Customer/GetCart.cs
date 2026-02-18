using UnityEngine;

public class GetCart : GAction
{

    private Customer customer;
    void Start()
    {
        preconditions.Clear();
        effects.Clear();
        preconditions["inStore"] = 1;
        effects["hasCart"] = 1;
    }

    public override bool PrePerform()
    {
        customer = GetComponent<Customer>();
        if (customer == null) return false;

        if (customer.HasCart)
        {
            beliefs.ModifyState("hasCart", 1);
            return false;
        }

        if (!SparkWorld.Instance.GetWorld().HasState("FreeCart"))
        {
            return false;
        }

        GameObject cart = SparkWorld.Instance.GetQueue("carts").RemoveResource();

        if (cart == null)
        {
            return false;
        }

        target = cart;
        SparkWorld.Instance.GetWorld().ModifyState("FreeCart", -1);

        return true;
    }

    public override bool PostPerform()
    {
        customer.AssignCart(target);
        inventory.AddItem(target);
        beliefs.ModifyState("hasCart", 1);
        return true;
    }
}