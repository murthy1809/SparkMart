using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Base class for all GOAP actions
/// </summary>
public abstract class GAction : MonoBehaviour {

    public string actionName = "Action";
    public float cost = 1.0f;
    public GameObject target;
    public string targetTag;
    public float duration = 0.0f;
    public WorldState[] preConditions;
    public WorldState[] afterEffects;
    public NavMeshAgent agent;

    public Dictionary<string, int> preconditions;
    public Dictionary<string, int> effects;

    public GInventory inventory;
    public WorldStates beliefs;

    public bool running = false;

    public GAction() {
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();
    }

    void Awake() {
        agent = GetComponent<NavMeshAgent>();

        if (preConditions != null) {
            foreach (WorldState w in preConditions) {
                preconditions.Add(w.key, w.value);
            }
        }

        if (afterEffects != null) {
            foreach (WorldState w in afterEffects) {
                effects.Add(w.key, w.value);
            }
        }

        GAgent gAgent = GetComponent<GAgent>();
        if (gAgent != null) {
            inventory = gAgent.inventory;
            beliefs = gAgent.beliefs;
        }
    }

    public bool IsAchievable() {
        return true;
    }

    public bool IsAchievableGiven(Dictionary<string, int> conditions) {
        foreach (KeyValuePair<string, int> p in preconditions) {
            if (!conditions.ContainsKey(p.Key)) {
                return false;
            }
        }
        return true;
    }

    public abstract bool PrePerform();
    public abstract bool PostPerform();
}
