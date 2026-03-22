using UnityEngine;

public class Sensor : MonoBehaviour, ISensor
{
    public string targetTag = "Player";
    public float detectionInterval = 0.1f;

    private Transform target;
    private float distanceToTarget;

    private void Start()
    {
        // Find target once (better than every check)
        GameObject obj = GameObject.FindGameObjectWithTag(targetTag);
        if (obj != null)
        {
            target = obj.transform;
        }

        // Start periodic sensing
        InvokeRepeating(nameof(UpdateSensor), 0f, detectionInterval);
    }

    void UpdateSensor()
    {
        if (target == null) return;

        distanceToTarget = Vector3.Distance(
            transform.position,
            target.position
        );
    }

    public float GetDistanceToTarget()
    {
        return distanceToTarget;
    }
}