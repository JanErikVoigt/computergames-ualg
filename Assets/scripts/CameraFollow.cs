using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Tracking")]
    public Transform target; 
    
    [Header("Camera Settings")]
    public float distanceBehind = 6f; // How far back the camera sits
    public float heightAbove = 3f;    // How high up the camera sits
    public float smoothSpeed = 5f; 

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate a position behind and above the target based on its current rotation
            Vector3 desiredPosition = target.position - (target.forward * distanceBehind) + (Vector3.up * heightAbove);
            
            // Smoothly move the camera
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Always look directly at the target
            transform.LookAt(target);
        }
    }

    public void SnapToTarget()
    {
        if (target != null)
        {
            transform.position = target.position - (target.forward * distanceBehind) + (Vector3.up * heightAbove);
            transform.LookAt(target);
        }
    }
}