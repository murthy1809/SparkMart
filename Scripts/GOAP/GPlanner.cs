using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Node in the GOAP planning graph
/// </summary>
public class Node {

    public Node parent;
    public float cost;
    public Dictionary<string, int> state;
    public GAction action;

    public Node(Node parent, float cost, Dictionary<string, int> allStates, GAction action) {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allStates);
        this.action = action;
    }

    public Node(Node parent, float cost, Dictionary<string, int> allStates, Dictionary<string, int> beliefStates, GAction action) {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allStates);

        foreach (KeyValuePair<string, int> b in beliefStates) {
            if (!this.state.ContainsKey(b.Key)) {
                this.state.Add(b.Key, b.Value);
            }
        }
        this.action = action;
    }
}

/// <summary>
/// GOAP Planner - builds action plans to achieve goals
/// </summary>
public class GPlanner {

    public Queue<GAction> Plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates beliefStates) {

        List<GAction> usableActions = new List<GAction>();

        foreach (GAction a in actions) {
            if (a.IsAchievable()) {
                usableActions.Add(a);
            }
        }

        // DEBUG
        //Debug.Log($"=== PLANNING ===");
        //Debug.Log($"Goal: {string.Join(", ", goal.Keys)}");
        //Debug.Log($"Available actions: {usableActions.Count}");
        //foreach (GAction a in usableActions) {
        //    Debug.Log($"  - {a.actionName}: preconditions={a.preconditions.Count}, effects={a.effects.Count}");
        //    foreach (var p in a.preconditions) Debug.Log($"      PRE: {p.Key}={p.Value}");
        //    foreach (var e in a.effects) Debug.Log($"      EFF: {e.Key}={e.Value}");
        //}
        //Debug.Log($"World states: {string.Join(", ", SparkWorld.Instance.GetWorld().GetStates().Keys)}");
        //Debug.Log($"Belief states: {string.Join(", ", beliefStates.GetStates().Keys)}");

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0.0f, SparkWorld.Instance.GetWorld().GetStates(), beliefStates.GetStates(), null);

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if (!success) {
            Debug.Log("PLAN FAILED - No valid plan found!");
            return null;
        }

        Node cheapest = null;
        foreach (Node leaf in leaves) {
            if (cheapest == null) {
                cheapest = leaf;
            } else if (leaf.cost < cheapest.cost) {
                cheapest = leaf;
            }
        }

        List<GAction> result = new List<GAction>();
        Node n = cheapest;

        while (n != null) {
            if (n.action != null) {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }

        Queue<GAction> queue = new Queue<GAction>();
        foreach (GAction a in result) {
            queue.Enqueue(a);
        }

        return queue;
    }

    bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal) {

        bool foundPath = false;

        foreach (GAction action in usableActions) {
            if (action.IsAchievableGiven(parent.state)) {

                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);

                foreach (KeyValuePair<string, int> eff in action.effects) {
                    if (!currentState.ContainsKey(eff.Key)) {
                        currentState.Add(eff.Key, eff.Value);
                    }
                }

                Node node = new Node(parent, parent.cost + action.cost, currentState, action);

                if (GoalAchieved(goal, currentState)) {
                    leaves.Add(node);
                    foundPath = true;
                } else {
                    List<GAction> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found) {
                        foundPath = true;
                    }
                }
            }
        }
        return foundPath;
    }

    List<GAction> ActionSubset(List<GAction> actions, GAction removeMe) {
        List<GAction> subset = new List<GAction>();
        foreach (GAction a in actions) {
            if (!a.Equals(removeMe)) {
                subset.Add(a);
            }
        }
        return subset;
    }

    bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state) {
        foreach (KeyValuePair<string, int> g in goal) {
            if (!state.ContainsKey(g.Key)) {
                return false;
            }
        }
        return true;
    }
}
