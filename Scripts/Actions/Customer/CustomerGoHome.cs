using UnityEngine;

public class CustomerGoHome : GAction
{

    private Customer customer;
    public string exitTag = "Exit";

    void Start()
    {
        preconditions.Clear();
        effects.Clear();
        preconditions["hasCheckedOut"] = 1;
        effects["leftStore"] = 1;
    }

    public override bool PrePerform()
    {
        customer = GetComponent<Customer>();
        if (customer == null) return false;

        GameObject exit = GameObject.FindWithTag(exitTag);

        if (exit == null)
        {
            exit = GameObject.FindWithTag("CustomerSpawn");
        }

        if (exit == null)
        {
            Debug.LogWarning("No exit found!");
            return false;
        }

        target = exit;
        return true;
    }

    public override bool PostPerform()
    {
        customer.LeaveStore();
        beliefs.ModifyState("leftStore", 1);
        return true;
    }
}