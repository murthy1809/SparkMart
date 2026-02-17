using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages multiple destination points on a shelf.
/// Attach to any shelf GameObject that has child transforms named "Destination", "Destination (1)", etc.
/// Tracks occupancy so multiple agents don't compete for the same spot.
/// </summary>
public class ShelfDestinationManager : MonoBehaviour
{

    [Header("Debug")]
    [SerializeField] private int totalSpots;
    [SerializeField] private int occupiedSpots;

    private List<DestinationSpot> spots = new List<DestinationSpot>();

    void Awake()
    {
        FindDestinationSpots();
    }

    void FindDestinationSpots()
    {
        spots.Clear();

        foreach (Transform child in transform)
        {
            // Match any child named "Destination", "Destination (1)", "Destination (2)", etc.
            if (child.name.StartsWith("Destination"))
            {
                spots.Add(new DestinationSpot
                {
                    transform = child,
                    isOccupied = false,
                    occupant = null
                });
            }
        }

        totalSpots = spots.Count;

        if (totalSpots == 0)
        {
            Debug.LogWarning($"ShelfDestinationManager on {gameObject.name}: No 'Destination' child transforms found! " +
                             "Add child GameObjects named 'Destination', 'Destination (1)', etc.");
        }
    }

    /// <summary>
    /// Gets the nearest available (unoccupied) destination for a given agent.
    /// Returns null if all spots are occupied.
    /// </summary>
    public Transform GetNearestAvailableSpot(Vector3 agentPosition)
    {
        Transform nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var spot in spots)
        {
            if (spot.isOccupied) continue;

            float dist = Vector3.Distance(agentPosition, spot.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = spot.transform;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Claims a destination spot for an agent. Returns true if successful.
    /// </summary>
    public bool ClaimSpot(Transform spotTransform, GameObject occupant)
    {
        for (int i = 0; i < spots.Count; i++)
        {
            if (spots[i].transform == spotTransform && !spots[i].isOccupied)
            {
                var spot = spots[i];
                spot.isOccupied = true;
                spot.occupant = occupant;
                spots[i] = spot;
                occupiedSpots++;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Releases a spot when an agent leaves.
    /// Call this when the agent finishes at the shelf.
    /// </summary>
    public void ReleaseSpot(GameObject occupant)
    {
        for (int i = 0; i < spots.Count; i++)
        {
            if (spots[i].occupant == occupant)
            {
                var spot = spots[i];
                spot.isOccupied = false;
                spot.occupant = null;
                spots[i] = spot;
                occupiedSpots--;
                return;
            }
        }
    }

    /// <summary>
    /// Releases a spot by its transform reference.
    /// </summary>
    public void ReleaseSpot(Transform spotTransform)
    {
        for (int i = 0; i < spots.Count; i++)
        {
            if (spots[i].transform == spotTransform)
            {
                var spot = spots[i];
                spot.isOccupied = false;
                spot.occupant = null;
                spots[i] = spot;
                occupiedSpots--;
                return;
            }
        }
    }

    public bool HasAvailableSpot()
    {
        foreach (var spot in spots)
        {
            if (!spot.isOccupied) return true;
        }
        return false;
    }

    public int AvailableSpotCount()
    {
        int count = 0;
        foreach (var spot in spots)
        {
            if (!spot.isOccupied) count++;
        }
        return count;
    }

    public int TotalSpotCount() => spots.Count;

    /// <summary>
    /// Gets the RestockPoint child transform (separate from customer destinations).
    /// Employees use this so they never compete with customers.
    /// </summary>
    public Transform GetRestockPoint()
    {
        Transform restockPoint = transform.Find("RestockPoint");
        if (restockPoint == null)
        {
            // Fallback: use the shelf's own position if no RestockPoint is set up
            Debug.LogWarning($"ShelfDestinationManager on {gameObject.name}: No 'RestockPoint' child found. " +
                             "Add a child GameObject named 'RestockPoint' for employees.");
        }
        return restockPoint;
    }

    void OnDrawGizmosSelected()
    {
        foreach (var spot in spots)
        {
            if (spot.transform == null) continue;
            Gizmos.color = spot.isOccupied ? Color.red : Color.cyan;
            Gizmos.DrawWireSphere(spot.transform.position, 0.3f);
        }

        Transform restock = transform.Find("RestockPoint");
        if (restock != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(restock.position, 0.35f);
        }
    }

    [System.Serializable]
    private struct DestinationSpot
    {
        public Transform transform;
        public bool isOccupied;
        public GameObject occupant;
    }
}