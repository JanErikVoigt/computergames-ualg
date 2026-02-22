using UnityEngine;
using UnityEngine.InputSystem;

public class Human : Driver
{
    public float slowSpeed = 4.0f;
    public float fastSpeed = 10.0f;
    public float lateralSpeed = 0.5f;

    public override Vector3 Move(float baseSpeed) // 'baseSpeed' parameter is defaultSpeed from MoveCar
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return Vector3.zero;
        }

        float currentForwardSpeed = 0f;

        if (keyboard.wKey.isPressed)
        {
            currentForwardSpeed = this.fastSpeed; // Move forward at fast speed
        }
        else if (keyboard.sKey.isPressed)
        {
            currentForwardSpeed = -this.slowSpeed; // Move backward at slow speed
        }

        float lateralVelocity = 0f;
        if (keyboard.aKey.isPressed)
        {
            // Lateral speed should be proportional to the absolute forward speed compared to the base speed
            lateralVelocity -= this.lateralSpeed * Mathf.Abs(currentForwardSpeed) / baseSpeed;
        }

        if (keyboard.dKey.isPressed)
        {
            lateralVelocity += this.lateralSpeed * Mathf.Abs(currentForwardSpeed) / baseSpeed;
        }

        return new Vector3(lateralVelocity, 0, currentForwardSpeed);
    }
}
