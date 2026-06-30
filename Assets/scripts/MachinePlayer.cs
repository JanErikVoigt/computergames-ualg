using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MachinePlayer : MonoBehaviour // Reverted back to MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 150f;

    [Header("Current Role & References")]
    public bool isCurrentlyActive = false; 
    public bool isHunter = false; 
    public Transform opponentTransform; 

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void ActivateAI(bool asHunter)
    {
        isCurrentlyActive = true;
        isHunter = asHunter;
    }

    public void DeactivateAI()
    {
        isCurrentlyActive = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        if (!isCurrentlyActive) return;

        // Custom standard AI movement logic goes here in the future
    }
}