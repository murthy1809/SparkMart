using UnityEngine;

public class CheckoutLane : MonoBehaviour {

    [Header("Configuration")]
    public int laneNumber = 1;
    public bool isSelfCheckout = false;

    [Header("State")]
    [SerializeField] private bool isStaffed = false;
    [SerializeField] private Employee assignedEmployee;

    public bool IsStaffed => isStaffed || isSelfCheckout;
    public Employee AssignedEmployee => assignedEmployee;

    public System.Action<CheckoutLane> OnLaneOpened;
    public System.Action<CheckoutLane> OnLaneClosed;

    public void AssignEmployee(Employee employee) {
        assignedEmployee = employee;
        isStaffed = true;
        OnLaneOpened?.Invoke(this);
    }

    public void RemoveEmployee() {
        assignedEmployee = null;
        isStaffed = false;
        OnLaneClosed?.Invoke(this);
    }

    void OnDrawGizmos() {
        Gizmos.color = IsStaffed ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, new Vector3(1f, 1f, 2f));
    }
}
