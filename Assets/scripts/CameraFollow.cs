using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Tracking")]
    public Transform target; 
    
    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 15, -15); 
    public float smoothSpeed = 5f; 

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate where the camera should be
            Vector3 desiredPosition = target.position + offset;
            
            // Smoothly slide the camera to that position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // --- THE FIX ---
            // This forces the camera to angle downwards and stare directly at the player
            transform.LookAt(target); 
        }
    }
}