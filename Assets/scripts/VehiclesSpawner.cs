using UnityEngine;

public class VehiclesSpawner : MonoBehaviour
{
    public GameObject[] vehicles;
    
    [Header("Slope Coordinates")]
    public Vector3 topLeftPos;      // Top of the slope
    public Vector3 bottomRightPos;  // Bottom of the slope

    void Start()
    {
        // e) Every 5 secs, spawn at a random X at top, move toward bottom
        InvokeRepeating("SpawnDown", 5f, 5f);

        // f) Every 10 secs, spawn at a random X at bottom, move toward top
        InvokeRepeating("SpawnUp", 10f, 10f);
    }
void SpawnDown()
{
    // 1. Pick a random X between the two points
    float randomX = Random.Range(topLeftPos.x, bottomRightPos.x);
    // Add +2 to Y so they fall from above the ramp
    Vector3 spawnPos = new Vector3(randomX, topLeftPos.y + 2f, topLeftPos.z);

    // 2. Calculate direction using only Y and Z (ignore X for rotation)
    Vector3 direction = new Vector3(0, bottomRightPos.y - topLeftPos.y, bottomRightPos.z - topLeftPos.z);
    Quaternion rotation = Quaternion.LookRotation(direction);

    // Pass negative speed to move towards bottom (assuming coordinate logic)
    addVehicles(spawnPos, -3f, rotation);
}

void SpawnUp()
{
    // 1. Pick a random X between the two points
    float randomX = Random.Range(topLeftPos.x, bottomRightPos.x);
    // Add +2 to Y so they fall from above the ramp
    Vector3 spawnPos = new Vector3(randomX, bottomRightPos.y + 2f, bottomRightPos.z);

    // 2. Calculate direction using only Y and Z (ignore X for rotation)
    Vector3 direction = new Vector3(0, topLeftPos.y - bottomRightPos.y, topLeftPos.z - bottomRightPos.z);
    Quaternion rotation = Quaternion.LookRotation(direction);

    // Pass positive speed to move towards top
    addVehicles(spawnPos, 3f, rotation);
}
    void addVehicles(Vector3 pos, float speed, Quaternion rot)
    {
        if (vehicles == null || vehicles.Length == 0) return;

        int v = Random.Range(0, vehicles.Length);
        GameObject vehicle = Instantiate(vehicles[v], pos, rot);
        
        Rigidbody rb = vehicle.GetComponent<Rigidbody>();
        if (rb == null) rb = vehicle.AddComponent<Rigidbody>();
        
        rb.useGravity = true;    
        rb.isKinematic = false;  
        
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Collider col = vehicle.GetComponent<Collider>();
        if (col == null)
        {
            // If prefab has no collider, add one. Adjust size if it feels "floating"
            BoxCollider bc = vehicle.AddComponent<BoxCollider>();
            bc.size = new Vector3(1.5f, 1.5f, 3f); // Approximate car size
        }

        UpDownCtl oldScript = vehicle.GetComponent<UpDownCtl>();
        if (oldScript != null) Destroy(oldScript);

        MoveCar moveScript = vehicle.AddComponent<MoveCar>();
        if (moveScript != null) {
            moveScript.defaultSpeed = speed;
            moveScript.moveOnlyAfterLanding = true;
            moveScript.fuel = 999999f; // Effectively infinite fuel
        }
    }
}
