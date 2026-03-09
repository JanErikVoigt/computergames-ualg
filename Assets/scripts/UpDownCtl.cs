using UnityEngine;

public class UpDownCtl : MonoBehaviour
{
    private float moveSpeed;
    private Rigidbody rb;
    private bool hasLanded = false;

    public void speed(float s)
    {
        moveSpeed = s;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Ensure we start with physics enabled so we can fall
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Once we hit the ramp (or anything solid), we stop being affected by physics
        // This makes the vehicle "unstoppable" by balls, but still solid for the player.
        if (!hasLanded)
        {
            hasLanded = true;
            if (rb != null)
            {
                rb.isKinematic = true; 
            }
        }
    }

    void Update()
    {
        // Only start driving once we've touched the ramp
        if (hasLanded)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }
}
