using UnityEngine;

public class Driver : MonoBehaviour, IDriver
{
    public virtual Vector3 Move(float speed)
    {
        return new Vector3(0, 0, speed);
    }
}
