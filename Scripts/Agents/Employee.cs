using UnityEngine;

public class Employee : GAgent {

    [Header("Employee Configuration")]
    public string employeeName = "Employee";
    public float dailySalary = 100f;

    [Header("Task Assignments")]
    public bool assignedToCheckout = true;
    public bool assignedToRestock = true;
    public bool assignedToHelp = true;
    public bool assignedToClean = true;

    [Header("Break Settings")]
    public float breakInterval = 300f;
    public float breakDuration = 30f;
    private float timeSinceLastBreak;

    [Header("Efficiency")]
    [SerializeField] private bool isTired = false;
    [SerializeField] private float efficiency = 1f;
    public float tiredEfficiencyMultiplier = 0.7f;

    [Header("State")]
    [SerializeField] private bool isOnBreak = false;
    [SerializeField] private bool isAtCheckoutLane = false;
    private GameObject assignedCheckoutLane;
    [Header("Carry Capacity")]
    public int maxCarryCapacity = 20;
    [SerializeField] private int currentCarrying = 0;

    public int CurrentCarrying => currentCarrying;

    public void PickUpStock(int amount)
    {
        currentCarrying = Mathf.Min(amount, maxCarryCapacity);
    }

    public int DeliverStock()
    {
        int delivered = currentCarrying;
        currentCarrying = 0;
        return delivered;
    }

    public bool HasStock => currentCarrying > 0;
    public bool IsTired => isTired;
    public float Efficiency => efficiency;
    public bool IsOnBreak => isOnBreak;
    public bool IsAtCheckoutLane => isAtCheckoutLane;
    public GameObject AssignedCheckoutLane => assignedCheckoutLane;

    public System.Action<Employee> OnBreakNeeded;
    public System.Action<Employee> OnBecameTired;

    public override void Start() {
        base.Start();
        timeSinceLastBreak = 0f;
        efficiency = 1f;
        SetupGoals();
    }

    void SetupGoals() {
        Debug.Log($"[{employeeName}] Setting up goals. Restock assigned: {assignedToRestock}, Checkout assigned: {assignedToCheckout}");
        SubGoal breakGoal = new SubGoal("rested", 1, false);
        goals.Add(breakGoal, 5);
        SubGoal patrolGoal = new SubGoal("patrolled", 1, false);
        goals.Add(patrolGoal, 1);
        if (assignedToCheckout) {
            SubGoal checkoutGoal = new SubGoal("operatingCheckout", 1, false);
            goals.Add(checkoutGoal, 3);
        }

        if (assignedToRestock) {
            SubGoal restockGoal = new SubGoal("shelvesRestocked", 1, false);
            goals.Add(restockGoal, 3);
        }

        if (assignedToHelp) {
            SubGoal helpGoal = new SubGoal("customerHelped", 1, false);
            goals.Add(helpGoal, 4);
        }

        if (assignedToClean) {
            SubGoal cleanGoal = new SubGoal("spillCleaned", 1, false);
            goals.Add(cleanGoal, 4);
        }
    }

    void Update() {
        if (!isOnBreak) {
            timeSinceLastBreak += Time.deltaTime;

if (timeSinceLastBreak >= breakInterval && !beliefs.HasState("needsBreak")) {
    NeedsBreak();
}
        }
    }

    void NeedsBreak() {
        beliefs.ModifyState("needsBreak", 1);
        OnBreakNeeded?.Invoke(this);
    }

    public void StartBreak() {
        isOnBreak = true;
        beliefs.ModifyState("isOnBreak", 1);

        if (isAtCheckoutLane) {
            LeaveCheckoutLane();
        }
    }

    public void FinishBreak() {
        isOnBreak = false;
        isTired = false;
        efficiency = 1f;
        timeSinceLastBreak = 0f;

        beliefs.RemoveState("isOnBreak");
        beliefs.RemoveState("needsBreak");
        beliefs.RemoveState("isTired");
    }

    public void SkipBreak() {
        isTired = true;
        efficiency = tiredEfficiencyMultiplier;
        beliefs.ModifyState("isTired", 1);
        beliefs.RemoveState("needsBreak");

        OnBecameTired?.Invoke(this);
    }

    public void AssignToCheckoutLane(GameObject lane) {
        assignedCheckoutLane = lane;
        isAtCheckoutLane = true;
        beliefs.ModifyState("atCheckoutLane", 1);

        SparkWorld.Instance.GetWorld().ModifyState("StaffedCheckoutLane", 1);
        SparkWorld.Instance.GetWorld().ModifyState("FreeCheckoutLane", -1);
    }

    public void LeaveCheckoutLane() {
        if (assignedCheckoutLane != null) {
            SparkWorld.Instance.GetQueue("checkoutLanes").AddResource(assignedCheckoutLane);
            SparkWorld.Instance.GetWorld().ModifyState("StaffedCheckoutLane", -1);
            SparkWorld.Instance.GetWorld().ModifyState("FreeCheckoutLane", 1);
        }

        assignedCheckoutLane = null;
        isAtCheckoutLane = false;
        beliefs.RemoveState("atCheckoutLane");
    }

    public void RestockShelf(Shelf shelf) {
        shelf.Restock();
    }

    public float GetAdjustedDuration(float baseDuration) {
        return baseDuration / efficiency;
    }

    public void UpdateAssignments(bool checkout, bool restock, bool help, bool clean) {
        assignedToCheckout = checkout;
        assignedToRestock = restock;
        assignedToHelp = help;
        assignedToClean = clean;

        goals.Clear();
        SetupGoals();
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = isTired ? Color.yellow : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
