using UnityEngine;

public class AI : Driver
{
    public override Vector3 Move(float speed)
    {
        // TODO: Implement AI driver logic
        return new Vector3(0, 0, speed); // For now, just move forward at default speed
    }
}
