using UnityEngine;

public class VehiclesSpawner : MonoBehaviour
{
    public GameObject[] vehicles;
    
    [Header("Slope Coordinates")]
    public Vector3 topLeftPos;      // Top of the slope
    public Vector3 bottomRightPos;  // Bottom of the slope

    [Header("Despawn Settings")]
    public float despawnYThreshold = -50f;

    void Start()
    {
        InvokeRepeating("SpawnDown", 5f, 5f);

        InvokeRepeating("SpawnUp", 10f, 10f);
    }
void SpawnDown()
{
    float randomX = Random.Range(topLeftPos.x, bottomRightPos.x);
    Vector3 spawnPos = new Vector3(randomX, topLeftPos.y + 2f, topLeftPos.z);

    Vector3 direction = new Vector3(0, bottomRightPos.y - topLeftPos.y, bottomRightPos.z - topLeftPos.z);
    Quaternion rotation = Quaternion.LookRotation(direction);

    addVehicles(spawnPos, -3f, rotation);
}

void SpawnUp()
{
    float randomX = Random.Range(topLeftPos.x, bottomRightPos.x);
    Vector3 spawnPos = new Vector3(randomX, bottomRightPos.y + 2f, bottomRightPos.z);

    Vector3 direction = new Vector3(0, topLeftPos.y - bottomRightPos.y, topLeftPos.z - bottomRightPos.z);
    Quaternion rotation = Quaternion.LookRotation(direction);

    addVehicles(spawnPos, 3f, rotation);
}
    void addVehicles(Vector3 pos, float speed, Quaternion rot)
    {
        if (vehicles == null || vehicles.Length == 0) return;

        int v = Random.Range(0, vehicles.Length);
        GameObject vehicle = Instantiate(vehicles[v], pos, rot);
        
        DespawnBelowY despawnScript = vehicle.AddComponent<DespawnBelowY>();
        despawnScript.threshold = despawnYThreshold;

        // Rigidbody rb = vehicle.GetComponent<Rigidbody>();
        // if (rb == null) rb = vehicle.AddComponent<Rigidbody>();
        
        // rb.useGravity = true;    
        // rb.isKinematic = false;  
        
        // rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Sensor sensorScript = vehicle.AddComponent<Sensor>(); 
        NPCMovement npcMovement = vehicle.AddComponent<NPCMovement>();
        IFuel fuelSystem = vehicle.AddComponent<InfiniteFuel>();

        MoveCar moveScript = vehicle.AddComponent<MoveCar>();
        if (moveScript != null) {
            moveScript.defaultSpeed = speed;
            moveScript.moveOnlyAfterLanding = true;
        }
    }
}
