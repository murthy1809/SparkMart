using UnityEngine;

public class EmployeePatrol : GAction {

    private Employee employee;

    void Start() {
        preconditions.Clear();
        effects.Clear();
        effects["patrolled"] = 1;
    }

    public override bool PrePerform() {
        employee = GetComponent<Employee>();
        if (employee == null) return false;

        // Don't patrol if there's real work to do
        if (employee.IsAtCheckoutLane) return false;
        if (employee.IsOnBreak) return false;

        // Pick a random stocked shelf to wander near
        Shelf randomShelf = SparkWorld.Instance.GetRandomStockedShelf();

        if (randomShelf == null) {
            // Fallback: find any shelf at all
            Shelf[] allShelves = Object.FindObjectsOfType<Shelf>();
            if (allShelves.Length == 0) return false;
            randomShelf = allShelves[Random.Range(0, allShelves.Length)];
        }

        target = randomShelf.gameObject;
        duration = Random.Range(3f, 6f); // Linger for a few seconds

        return true;
    }

    public override bool PostPerform() {
        beliefs.ModifyState("patrolled", 1);
        beliefs.RemoveState("patrolled");
        return true;
    }
}
