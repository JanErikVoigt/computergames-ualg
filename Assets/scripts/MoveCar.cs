using TMPro;
using UnityEngine;

[RequireComponent(typeof(Driver))]
[RequireComponent(typeof(Rigidbody))]
public class MoveCar : MonoBehaviour
{
    public float steerIntensity = 10.0f;

    public float defaultSpeed = 6.0f;
    public float fuel = 100f;
    public float fuelConsumptionRate = 1f;

    [SerializeField] private Transform truckFront;
    public TextMeshProUGUI fuelDisplay;
    public string textPrefix;

    private IDriver driver;
    private Rigidbody rb;
    private int hits = 0;

    void Awake()
    {
        driver = GetComponent<IDriver>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (fuelDisplay != null)
        {
            fuelDisplay.text = $"{textPrefix} Fuel: " + Mathf.Max(0, (int)fuel).ToString();
        }
    }

    void FixedUpdate()
    {
        if (driver == null) return;

        Vector3 moveVelocity = Vector3.zero;

        if (fuel > 0)
        {
            moveVelocity = driver.Move(defaultSpeed);

            float fuelConsumed = (moveVelocity.magnitude / defaultSpeed) * fuelConsumptionRate * Time.fixedDeltaTime;
            fuel -= fuelConsumed;
            if (fuel < 0) fuel = 0;
        }

        float lateralVelocity = moveVelocity.x;
        float speed = moveVelocity.z;

        rb.velocity = new Vector3(-lateralVelocity, rb.velocity.y, -speed);

        if (truckFront != null)
        {
            truckFront.localRotation = Quaternion.Euler(0f, 90f + lateralVelocity * steerIntensity, 0f);
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "hits: " + hits);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") || collision.gameObject.name.Contains("Ball") || collision.gameObject.name.Contains("Bus") || collision.gameObject.name.Contains("Car") || collision.gameObject.name.Contains("Police"))
        {
            hits++;
        }
    }
}
