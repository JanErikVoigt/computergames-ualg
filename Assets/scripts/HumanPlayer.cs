using UnityEngine;

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
    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isCurrentlyActive)
        {
            // NEW: Using GetAxisRaw completely eliminates tiny phantom stick drift
            moveInput = Input.GetAxisRaw("Vertical");   
            turnInput = Input.GetAxisRaw("Horizontal"); 
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