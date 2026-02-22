using UnityEngine;

public class Driver : MonoBehaviour, IDriver
{
    public virtual Vector3 Move(float speed)
    {
        return Vector3.zero;
    }
}
