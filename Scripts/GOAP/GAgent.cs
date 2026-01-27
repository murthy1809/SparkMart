using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Subgoal for GOAP agents
/// </summary>
public class SubGoal {
    public Dictionary<string, int> sGoals;
    public bool remove;

    public SubGoal(string key, int value, bool remove) {
        sGoals = new Dictionary<string, int>();
        sGoals.Add(key, value);
        this.remove = remove;
    }
}

/// <summary>
/// Base GOAP Agent class
/// </summary>
public class GAgent : MonoBehaviour {

    public List<GAction> actions = new List<GAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    public GInventory inventory = new GInventory();
    public WorldStates beliefs = new WorldStates();

    GPlanner planner;
    Queue<GAction> actionQueue;
    public GAction currentAction;
    SubGoal currentGoal;

    Vector3 destination = Vector3.zero;
    protected bool invoked = false;

    public virtual void Start() {
        GAction[] acts = GetComponents<GAction>();
        foreach (GAction a in acts) {
            actions.Add(a);
        }
    }

    void CompleteAction() {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
    }

    void LateUpdate() {
        if (currentAction != null && currentAction.running) {
            float distanceToTarget = Vector3.Distance(destination, transform.position);
            
            if (distanceToTarget < 2f) {
                if (!invoked) {
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }
            return;
        }

        if (planner == null || actionQueue == null) {
            planner = new GPlanner();

            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            foreach (KeyValuePair<SubGoal, int> sg in sortedGoals) {
                actionQueue = planner.Plan(actions, sg.Key.sGoals, beliefs);
                if (actionQueue != null) {
                    currentGoal = sg.Key;
                    break;
                }
            }
        }

        if (actionQueue != null && actionQueue.Count == 0) {
            if (currentGoal.remove) {
                goals.Remove(currentGoal);
            }
            planner = null;
        }

        if (actionQueue != null && actionQueue.Count > 0) {
            currentAction = actionQueue.Dequeue();
            
            if (currentAction.PrePerform()) {
                if (currentAction.target == null && currentAction.targetTag != "") {
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }

                if (currentAction.target != null) {
                    currentAction.running = true;
                    destination = currentAction.target.transform.position;
                    
                    Transform dest = currentAction.target.transform.Find("Destination");
                    if (dest != null) {
                        destination = dest.position;
                    }

                    currentAction.agent.SetDestination(destination);
                }
            } else {
                actionQueue = null;
            }
        }
    }
}
