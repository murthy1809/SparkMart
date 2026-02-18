using UnityEngine;

/// <summary>
/// Forces customers to enter the store through a designated entry point.
/// Place EntryPoint GameObjects at the store entrance to control flow direction.
/// Supports multiple entry points — picks the nearest one.
/// </summary>
public class GoToStore : GAction {

    void Start() {
        actionName = "GoToStore";
        preconditions.Clear();
        effects.Clear();
        effects["inStore"] = 1;
    }

    public override bool PrePerform() {
        GameObject[] entryPoints = GameObject.FindGameObjectsWithTag("EntryPoint");

        if (entryPoints.Length == 0) {
            Debug.LogWarning("No EntryPoint found! Add GameObjects tagged 'EntryPoint' at store entrances.");
            // Skip this action gracefully — let customer proceed without entry routing
            beliefs.ModifyState("inStore", 1);
            return false;
        }

        // Pick the nearest entry point
        GameObject nearest = null;
        float nearestDist = float.MaxValue;

        foreach (GameObject ep in entryPoints) {
            float dist = Vector3.Distance(transform.position, ep.transform.position);
            if (dist < nearestDist) {
                nearestDist = dist;
                nearest = ep;
            }
        }

        // Pick a random entry point
        target = entryPoints[Random.Range(0, entryPoints.Length)];
        duration = 0.5f;

        return true;
    }

    public override bool PostPerform() {
        beliefs.ModifyState("inStore", 1);
        return true;
    }
}
