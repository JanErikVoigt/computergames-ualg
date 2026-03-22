using UnityEngine;

[RequireComponent(typeof(Sensor))]
public class NPCMovement : MonoBehaviour, IDriver
{
    public float followDistance = 10.0f;

    private IMovementStrategy currentStrategy;
    private Sensor sensor;
    private Human humanTruck;
    private Vector3 lastMovement;

    void Start()
    {
        sensor = GetComponent<Sensor>();
        humanTruck = Object.FindObjectOfType<Human>();
    }

    public Vector3 Move(float baseSpeed)
    {
        if (sensor == null)
        {
            sensor = GetComponent<Sensor>();
            if (sensor == null) return new Vector3(0, 0, baseSpeed);
        }

        if (humanTruck == null)
        {
            humanTruck = Object.FindObjectOfType<Human>();
            if (humanTruck == null) return new Vector3(0, 0, baseSpeed);
        }
        // Initialize strategy if not set (initial state: maintain normal direction)
        if (currentStrategy == null)
        {
            lastMovement = new Vector3(0, 0, baseSpeed);
            currentStrategy = new MaintainDirectionStrategy(lastMovement);
        }

        float distance = sensor.GetDistanceToTarget();

        // Strategy switching logic
        if (distance > 0 && distance <= followDistance)
        {
            if (currentStrategy is not FollowTargetStrategy)
            {
                currentStrategy = new FollowTargetStrategy();
            }
        }
        else
        {
            // When leaving range, maintain the last movement vector calculated while in range
            if (currentStrategy is not MaintainDirectionStrategy)
            {
                currentStrategy = new MaintainDirectionStrategy(lastMovement);
            }
        }

        Vector3 move = currentStrategy.GetMovement(transform.position, humanTruck.transform, baseSpeed);

        // Update lastMovement if we are currently following, so it's ready when we leave range
        if (currentStrategy is FollowTargetStrategy)
        {
            lastMovement = move;
        }

        return move;
    }
}
