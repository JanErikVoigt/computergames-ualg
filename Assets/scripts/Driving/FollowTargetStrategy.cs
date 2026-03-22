using UnityEngine;

public class FollowTargetStrategy : IMovementStrategy
{
    public Vector3 GetMovement(Vector3 currentPosition, Transform target, float speed)
    {
        if (target == null) return new Vector3(0, 0, speed);

        // Direction in world space from current position to target
        Vector3 direction = (target.position - currentPosition).normalized;

        // MoveCar logic: rb.linearVelocity = (-move.x, y, -move.z)
        // To move with velocity V = direction * |speed|, we need:
        // -move.x = V.x  => move.x = -V.x
        // -move.z = V.z  => move.z = -V.z
        
        float absoluteSpeed = Mathf.Abs(speed);
        return new Vector3(-direction.x * absoluteSpeed, 0, -direction.z * absoluteSpeed);
    }
}
