using UnityEngine;

public class AI : Driver
{
    public override Vector3 Move(float baseSpeed)
    {
        // Simple default forward movement
        return new Vector3(0, 0, baseSpeed);
    }
}
