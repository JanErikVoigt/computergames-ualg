using UnityEngine;

public interface IMovementStrategy
{
    Vector3 GetMovement(Vector3 currentPosition, Transform target, float speed);
}
