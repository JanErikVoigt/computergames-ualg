using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class HumanPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 150f;

    [Header("Current Role")]
    public bool isCurrentlyActive = false; 
    public bool isHunter = false; 

    private Rigidbody rb;
    private PlayerInput playerInput; // 1. Add a reference to the PlayerInput component
    private Vector2 rawInput = Vector2.zero;
    private float moveInput;
    private float turnInput;

    void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        // 2. Grab the component when the game starts
        playerInput = GetComponent<PlayerInput>(); 
    }

    public void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    void Update()
    {
        // 3. Automatically turn the Input System on/off based on the active state
        if (playerInput != null && playerInput.enabled != isCurrentlyActive)
        {
            playerInput.enabled = isCurrentlyActive;
        }

        if (isCurrentlyActive)
        {
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
            rawInput = Vector2.zero; // Clear out any residual input when deactivated
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}