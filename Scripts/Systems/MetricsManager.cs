using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MetricsManager : MonoBehaviour {

    [Header("Current Metrics")]
    [SerializeField] private int totalCustomersServed = 0;
    [SerializeField] private float totalProfit = 0f;
    [SerializeField] private int totalItemsSold = 0;
    [SerializeField] private float averageSatisfaction = 0f;
    [SerializeField] private int totalThefts = 0;
    [SerializeField] private int totalAccidents = 0;
    [SerializeField] private float securityRating = 100f;
    [SerializeField] private float accidentRate = 0f;

    [Header("Settings")]
    public float snapshotInterval = 60f;
    public int maxSnapshots = 1440;

    private List<float> satisfactionScores = new List<float>();
    private List<MetricsSnapshot> snapshots = new List<MetricsSnapshot>();
    private float lastSnapshotTime = 0f;

    public int TotalCustomersServed => totalCustomersServed;
    public float TotalProfit => totalProfit;
    public int TotalItemsSold => totalItemsSold;
    public float AverageSatisfaction => averageSatisfaction;
    public float SecurityRating => securityRating;
    public float AccidentRate => accidentRate;

    public System.Action OnMetricsUpdated;

    void Update() {
        if (Time.time - lastSnapshotTime >= snapshotInterval) {
            TakeSnapshot();
            lastSnapshotTime = Time.time;
        }
    }

    public void RecordSale(float profit, int itemCount) {
        totalCustomersServed++;
        totalProfit += profit;
        totalItemsSold += itemCount;
        UpdateMetrics();
        OnMetricsUpdated?.Invoke();
    }

    public void RecordCustomerSatisfaction(float satisfaction) {
        satisfactionScores.Add(satisfaction);

        float sum = 0f;
        foreach (float score in satisfactionScores) {
            sum += score;
        }
        averageSatisfaction = sum / satisfactionScores.Count;

        UpdateMetrics();
        OnMetricsUpdated?.Invoke();
    }

    public void RecordTheft() {
        totalThefts++;
        UpdateSecurityRating();
        OnMetricsUpdated?.Invoke();
    }

    public void RecordAccident() {
        totalAccidents++;
        UpdateAccidentRate();
        OnMetricsUpdated?.Invoke();
    }

    void UpdateMetrics() {
        UpdateSecurityRating();
        UpdateAccidentRate();
    }

    void UpdateSecurityRating() {
        if (totalCustomersServed == 0) {
            securityRating = 100f;
            return;
        }
        float theftPercentage = (float)totalThefts / totalCustomersServed * 100f;
        securityRating = Mathf.Max(0f, 100f - theftPercentage);
    }

    void UpdateAccidentRate() {
        if (totalCustomersServed == 0) {
            accidentRate = 0f;
            return;
        }
        accidentRate = (float)totalAccidents / totalCustomersServed * 100f;
    }

    public void TakeSnapshot() {
        MetricsSnapshot snapshot = new MetricsSnapshot {
            timestamp = Time.time,
            customersServed = totalCustomersServed,
            profit = totalProfit,
            itemsSold = totalItemsSold,
            satisfaction = averageSatisfaction,
            thefts = totalThefts,
            accidents = totalAccidents,
            securityRating = securityRating,
            accidentRate = accidentRate
        };

        snapshots.Add(snapshot);

        while (snapshots.Count > maxSnapshots) {
            snapshots.RemoveAt(0);
        }
    }

    public List<MetricsSnapshot> GetSnapshots() {
        return new List<MetricsSnapshot>(snapshots);
    }

    public void ExportToCSV(string filename = null) {
        if (string.IsNullOrEmpty(filename)) {
            filename = $"SparkMart_Metrics_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
        }

        string path = Path.Combine(Application.persistentDataPath, filename);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("timestamp,customers_served,profit,items_sold,satisfaction,thefts,accidents,security_rating,accident_rate");

        foreach (var snapshot in snapshots) {
            sb.AppendLine($"{snapshot.timestamp:F2},{snapshot.customersServed},{snapshot.profit:F2},{snapshot.itemsSold},{snapshot.satisfaction:F2},{snapshot.thefts},{snapshot.accidents},{snapshot.securityRating:F2},{snapshot.accidentRate:F2}");
        }

        File.WriteAllText(path, sb.ToString());
        Debug.Log($"Metrics exported to: {path}");
    }

    public void ResetMetrics() {
        totalCustomersServed = 0;
        totalProfit = 0f;
        totalItemsSold = 0;
        averageSatisfaction = 0f;
        totalThefts = 0;
        totalAccidents = 0;
        securityRating = 100f;
        accidentRate = 0f;

        satisfactionScores.Clear();
        snapshots.Clear();

        OnMetricsUpdated?.Invoke();
    }

    public string GetSummary() {
        return $"Customers: {totalCustomersServed}\n" +
               $"Profit: ${totalProfit:F2}\n" +
               $"Items Sold: {totalItemsSold}\n" +
               $"Satisfaction: {averageSatisfaction:F1}%\n" +
               $"Security: {securityRating:F1}%\n" +
               $"Accidents: {accidentRate:F1}%";
    }
}

[System.Serializable]
public struct MetricsSnapshot {
    public float timestamp;
    public int customersServed;
    public float profit;
    public int itemsSold;
    public float satisfaction;
    public int thefts;
    public int accidents;
    public float securityRating;
    public float accidentRate;
}
