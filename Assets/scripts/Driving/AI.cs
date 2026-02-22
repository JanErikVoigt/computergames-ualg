using UnityEngine;

public class AI : Driver
{
    public float speed = 6.0f;

    public override Vector3 Move(float speed)
    {
        return new Vector3(0, 0, this.speed);
    }
}
