using UnityEngine;
using UnityEngine.InputSystem;

public class Human : Driver
{
    public float slowSpeed = 4.0f;
    public float defaultSpeed = 6.0f;
    public float fastSpeed = 10.0f;
    public float lateralSpeed = 0.5f;


    public override Vector3 Move(float speed)
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return Vector3.zero;
        }

        // Forward speed control. 'speed' parameter is the default speed.
        float currentSpeed = speed;

        if (keyboard.wKey.isPressed)
        {
            currentSpeed = this.fastSpeed;
        }

        if (keyboard.sKey.isPressed)
        {
            currentSpeed = this.slowSpeed;
        }

        // Sideways movement
        float lateralVelocity = 0f;
        if (keyboard.aKey.isPressed)
        {
            lateralVelocity -= this.lateralSpeed * currentSpeed / speed;
        }

        if (keyboard.dKey.isPressed)
        {
            lateralVelocity += this.lateralSpeed * currentSpeed / speed;
        }

        return new Vector3(lateralVelocity, 0, currentSpeed);
    }
}
