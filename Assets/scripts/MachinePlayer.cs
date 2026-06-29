using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody))]
public class MachinePlayer : Agent // Inherit from Agent instead of MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 150f;

    [Header("Current Role & References")]
    public bool isCurrentlyActive = false; 
    public bool isHunter = false; 
    public Transform opponentTransform; // The AI needs to know where the human is

    private Rigidbody rb;

    public override void Initialize()
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
        // Stop any residual movement when deactivated
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // 1. OBSERVATIONS: What the AI "sees"
    public override void CollectObservations(VectorSensor sensor)
    {
        if (!isCurrentlyActive) return;

        // Is it the hunter? (1 value)
        sensor.AddObservation(isHunter ? 1f : 0f);

        // Own position and velocity (3 + 3 = 6 values)
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(rb.linearVelocity);

        // Opponent position (3 values)
        if (opponentTransform != null)
        {
            sensor.AddObservation(opponentTransform.localPosition);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
        }
        
        // Total Observations Space Size = 10
    }

    // 2. ACTIONS: How the AI moves
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!isCurrentlyActive) return;

        // Extract continuous actions (from -1.0 to 1.0)
        float moveInput = actions.ContinuousActions[0]; 
        float turnInput = actions.ContinuousActions[1]; 

        // Apply movement
        Vector3 movement = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Apply rotation
        Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // Time-step rewards to encourage active gameplay
        if (isHunter)
        {
            // Tiny penalty each step encourages hunting quickly
            AddReward(-1f / MaxStep);
        }
        else
        {
            // Tiny reward each step encourages staying alive
            AddReward(1f / MaxStep);
        }
    }

    // 3. HEURISTIC: Allows manual testing with a keyboard before training
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");   // W/S keys
        continuousActionsOut[1] = Input.GetAxis("Horizontal"); // A/D keys
    }
}