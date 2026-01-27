using UnityEngine;

public class TakeBreak : GAction {

    private Employee employee;

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

        beliefs.ModifyState("rested", 1);
        beliefs.RemoveState("needsBreak");
        beliefs.RemoveState("rested");

        return true;
    }
}
