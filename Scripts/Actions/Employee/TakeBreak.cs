using UnityEngine;

public class TakeBreak : GAction {

    private Employee employee;

    void Start() {
        actionName = "TakeBreak";
        
        // Preconditions: needs break
        preconditions.Clear();
        preconditions.Add("needsBreak", 1);
        
        // Effects: rested
        effects.Clear();
        effects.Add("rested", 1);
    }

    public override bool PrePerform() {
        employee = GetComponent<Employee>();
        if (employee == null) return false;

        if (!SparkWorld.Instance.GetWorld().HasState("FreeBreakRoom")) {
            employee.SkipBreak();
            return false;
        }

        GameObject breakSpot = SparkWorld.Instance.GetQueue("breakRoom").RemoveResource();

        if (breakSpot == null) {
            employee.SkipBreak();
            return false;
        }

        target = breakSpot;
        SparkWorld.Instance.GetWorld().ModifyState("FreeBreakRoom", -1);
        duration = employee.breakDuration;

        return true;
    }

    public override bool PostPerform() {
        SparkWorld.Instance.GetQueue("breakRoom").AddResource(target);
        SparkWorld.Instance.GetWorld().ModifyState("FreeBreakRoom", 1);

        employee.FinishBreak();

        beliefs.RemoveState("needsBreak");

        return true;
    }
}
