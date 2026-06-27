using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class HumanPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 150f;

    [Header("Current Role")]
    public bool isCurrentlyActive = false; // Is the human controlling this capsule?
    public bool isHunter = false; // True = Hunter, False = Runner

    private Rigidbody rb;
    private Vector2 rawInput = Vector2.zero;
    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    /// <summary>
    /// Callback sent by the PlayerInput component when a Move action is triggered.
    /// Requires the PlayerInput behavior to be set to "Send Messages" or "Broadcast Messages".
    /// </summary>
    public void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    void Update()
    {
        if (isCurrentlyActive)
        {
            // Apply standard stick deadzone in code just in case the action map lacks one
            Vector2 processedInput = rawInput;
            if (processedInput.magnitude <= 0.15f)
            {
                processedInput = Vector2.zero;
            }

            moveInput = processedInput.y;
            turnInput = processedInput.x;
        }
        else
        {
            moveInput = 0f;
            turnInput = 0f;
        }
    }

    void FixedUpdate()
    {
        // Apply forward/backward movement
        Vector3 movement = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Apply rotation
        Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}