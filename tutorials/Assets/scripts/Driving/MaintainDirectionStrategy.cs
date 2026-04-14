using UnityEngine;

public class MaintainDirectionStrategy : IMovementStrategy
{
    private Vector3 direction;

    public MaintainDirectionStrategy(Vector3 direction)
    {
        this.direction = direction;
    }

    public Vector3 GetMovement(Vector3 currentPosition, Transform target, float speed)
    {
        return this.direction;
    }
}
