using UnityEngine;
using UnityEngine.InputSystem;

public class Human : MonoBehaviour, IDriver
{
    public float slowSpeed = 4.0f;
    public float fastSpeed = 10.0f;
    public float lateralSpeed = 0.5f;

    private Vector2 moveInput;

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public Vector3 Move(float baseSpeed)
    {
        float currentForwardSpeed = 0f;

        // W key pressed (forward)
        if (moveInput.y > 0.5f)
        {
            currentForwardSpeed = this.fastSpeed; // Move forward at fast speed
        }
        // S key pressed (backward)
        else if (moveInput.y < -0.5f)
        {
            currentForwardSpeed = -this.slowSpeed; // Move backward at slow speed
        }

        float lateralVelocity = 0f;
        // A key pressed (left)
        if (moveInput.x < -0.5f)
        {
            // Lateral speed should be proportional to the absolute forward speed compared to the base speed
            lateralVelocity -= this.lateralSpeed * Mathf.Abs(currentForwardSpeed) / baseSpeed;
        }
        // D key pressed (right)
        if (moveInput.x > 0.5f)
        {
            lateralVelocity += this.lateralSpeed * Mathf.Abs(currentForwardSpeed) / baseSpeed;
        }

        return new Vector3(lateralVelocity, 0, currentForwardSpeed);
    }
}
