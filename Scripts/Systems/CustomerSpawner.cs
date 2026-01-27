using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour {

    [Header("Spawn Configuration")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 5f;
    public float spawnIntervalVariance = 2f;
    public bool autoSpawn = true;
    public int maxCustomers = 50;

    [Header("Personas")]
    public List<PersonaSpawnConfig> personaConfigs = new List<PersonaSpawnConfig>();

    [Header("Statistics")]
    [SerializeField] private int totalSpawned = 0;
    [SerializeField] private int currentCustomers = 0;

    private List<Customer> activeCustomers = new List<Customer>();
    private int totalSpawnWeight = 0;

    void Start() {
        CalculateTotalWeight();

        if (autoSpawn) {
            SpawnCustomer();
            ScheduleNextSpawn();
        }
    }

    void CalculateTotalWeight() {
        totalSpawnWeight = 0;
        foreach (var config in personaConfigs) {
            if (config.persona != null && config.enabled) {
                totalSpawnWeight += config.spawnWeight;
            }
        }
    }

    void ScheduleNextSpawn() {
        float nextSpawn = spawnInterval + Random.Range(-spawnIntervalVariance, spawnIntervalVariance);
        nextSpawn = Mathf.Max(1f, nextSpawn);
        Invoke(nameof(SpawnAndSchedule), nextSpawn);
    }

    void SpawnAndSchedule() {
        if (autoSpawn && currentCustomers < maxCustomers) {
            SpawnCustomer();
        }
        ScheduleNextSpawn();
    }

    public Customer SpawnCustomer() {
        if (customerPrefab == null) {
            Debug.LogError("Customer prefab not assigned!");
            return null;
        }

        if (currentCustomers >= maxCustomers) {
            return null;
        }

        CustomerPersona selectedPersona = SelectRandomPersona();

        if (selectedPersona == null) {
            Debug.LogWarning("No persona available!");
            return null;
        }

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        spawnPos += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        GameObject customerObj = Instantiate(customerPrefab, spawnPos, Quaternion.identity);
        customerObj.name = $"Customer_{totalSpawned}_{selectedPersona.personaName}";

        Customer customer = customerObj.GetComponent<Customer>();
        if (customer != null) {
            customer.persona = selectedPersona;
            customer.OnLeftStore += OnCustomerLeft;

            activeCustomers.Add(customer);
            totalSpawned++;
            currentCustomers++;
        }

        return customer;
    }

    CustomerPersona SelectRandomPersona() {
        if (totalSpawnWeight <= 0) {
            CalculateTotalWeight();
            if (totalSpawnWeight <= 0) return null;
        }

        int roll = Random.Range(0, totalSpawnWeight);
        int cumulative = 0;

        foreach (var config in personaConfigs) {
            if (config.persona != null && config.enabled) {
                cumulative += config.spawnWeight;
                if (roll < cumulative) {
                    return config.persona;
                }
            }
        }

        foreach (var config in personaConfigs) {
            if (config.persona != null && config.enabled) {
                return config.persona;
            }
        }

        return null;
    }

    void OnCustomerLeft(Customer customer) {
        activeCustomers.Remove(customer);
        currentCustomers--;
    }

    public void SetPersonaSpawnWeight(PersonaType personaType, int weight) {
        foreach (var config in personaConfigs) {
            if (config.persona != null && config.persona.personaType == personaType) {
                config.spawnWeight = Mathf.Max(0, weight);
                break;
            }
        }
        CalculateTotalWeight();
    }

    public int GetCurrentCustomerCount() => currentCustomers;
    public int GetTotalSpawned() => totalSpawned;
}

[System.Serializable]
public class PersonaSpawnConfig {
    public CustomerPersona persona;
    public int spawnWeight = 25;
    public bool enabled = true;
}
