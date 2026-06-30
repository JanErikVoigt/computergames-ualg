using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class MachinePlayer : MonoBehaviour
{
    [Header("Current Role")]
    public bool isCurrentlyActive = false; // Is the AI controlling this capsule?
    public bool isHunter = false; // True = Hunter, False = Runner

    [Header("AI Settings")]
    public float agentSpeed = 10f;
    public float agentAcceleration = 20f;
    public float agentAngularSpeed = 360f;

    [Header("Hiding Settings")]
    public float hideRadius = 30f;
    public float hideOffset = 2.5f;
    public float pathUpdateInterval = 0.1f;
    public LayerMask obstacleLayerMask = ~0; // Target all layers by default, excluding player layers is recommended in Unity

    private NavMeshAgent agent;
    private Rigidbody rb;
    private float updateTimer = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // Initially disable NavMeshAgent to avoid conflict with human controls
        if (agent != null)
        {
            agent.enabled = false;
        }
    }

    public void ActivateAI(bool asHunter)
    {
        isCurrentlyActive = true;
        isHunter = asHunter;

        // Set Rigidbody to Kinematic so physics doesn't conflict with NavMesh steering
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Enable NavMeshAgent and configure parameters
        if (agent != null)
        {
            agent.enabled = true;
            agent.speed = agentSpeed;
            agent.acceleration = agentAcceleration;
            agent.angularSpeed = agentAngularSpeed;

            // Force immediate destination update
            updateTimer = pathUpdateInterval;
        }
    }

    public void DeactivateAI()
    {
        isCurrentlyActive = false;

        // Disable NavMeshAgent
        if (agent != null)
        {
            agent.enabled = false;
        }

        // Re-enable Rigidbody physics for human player control
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    /// <summary>
    /// Safely warps the agent to a position without breaking the NavMeshAgent tracking.
    /// </summary>
    public void Warp(Vector3 position, Quaternion rotation)
    {
        if (agent != null && agent.enabled)
        {
            agent.Warp(position);
        }
        else
        {
            transform.position = position;
        }
        transform.rotation = rotation;

        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (!isCurrentlyActive) return;

        // Sync speed directly in update to prevent NavMeshAgent initialization from resetting it to inspector defaults
        if (agent != null && agent.enabled && agent.speed != agentSpeed)
        {
            agent.speed = agentSpeed;
        }

        updateTimer += Time.deltaTime;
        if (updateTimer >= pathUpdateInterval)
        {
            updateTimer = 0f;
            UpdateAIDestination();
        }
    }

    private void UpdateAIDestination()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        if (isHunter)
        {
            Transform runner = GameManager.Instance.runnerTransform;
            if (runner != null)
            {
                agent.SetDestination(runner.position);
            }
        }
        else
        {
            Transform hunter = GameManager.Instance.hunterTransform;
            if (hunter != null)
            {
                Vector3 targetDest = FindBestHidingSpot(hunter);
                agent.SetDestination(targetDest);
            }
        }
    }

    private Vector3 FindBestHidingSpot(Transform hunter)
    {
        // Find potential obstacles in the area
        Collider[] colliders = Physics.OverlapSphere(transform.position, hideRadius, obstacleLayerMask);

        Vector3 bestSpot = transform.position;
        float bestScore = float.MinValue;
        bool foundHidingSpot = false;

        foreach (var col in colliders)
        {
            // Skip player and opponent capsules
            if (col.transform == transform || col.transform == hunter)
                continue;

            // Skip triggers
            if (col.isTrigger)
                continue;

            // Skip ground/plane
            if (col.bounds.size.y < 0.2f && col.bounds.size.x > 15f)
                continue;

            Vector3 obstaclePos = col.bounds.center;
            Vector3 dirFromHunter = (obstaclePos - hunter.position).normalized;

            // Hiding point is on the opposite side of the obstacle center relative to the hunter
            Vector3 candidatePos = obstaclePos + dirFromHunter * (col.bounds.extents.magnitude + hideOffset);

            // Sample on NavMesh to ensure it is traversable
            if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                Vector3 spot = hit.position;

                // Linecast to verify if Hunter's line of sight to the hiding spot is blocked
                Vector3 hunterEye = hunter.position + Vector3.up * 1.5f;
                Vector3 spotEye = spot + Vector3.up * 1.5f;

                bool hitSomething = Physics.Linecast(hunterEye, spotEye, out RaycastHit lineHit, obstacleLayerMask);

                // Spot is hidden if linecast hit an obstacle that is not the runner/hunter
                bool isHidden = hitSomething && lineHit.transform != transform && lineHit.transform != hunter;

                // Score this spot
                float distFromHunter = Vector3.Distance(spot, hunter.position);
                float distFromRunner = Vector3.Distance(spot, transform.position);

                float score = 0f;
                if (isHidden)
                {
                    score += 1000f; // Strongly prioritize spots that block vision
                }

                score += distFromHunter * 1.5f; // Favor being far from the hunter
                score -= distFromRunner * 0.5f; // Favor closer hiding spots to minimize travel risk

                if (score > bestScore)
                {
                    bestScore = score;
                    bestSpot = spot;
                    foundHidingSpot = true;
                }
            }
        }

        if (foundHidingSpot)
        {
            return bestSpot;
        }

        // Fallback: If no obstacles, flee directly away from the hunter
        Vector3 fleeDir = (transform.position - hunter.position).normalized;
        Vector3 fleePos = transform.position + fleeDir * 10f;
        if (NavMesh.SamplePosition(fleePos, out NavMeshHit fallbackHit, 10f, NavMesh.AllAreas))
        {
            return fallbackHit.position;
        }

        return transform.position;
    }
}