using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Driver))]
[RequireComponent(typeof(Rigidbody))]
public class MoveCar : MonoBehaviour
{
    public float steerIntensity = 10.0f;

    public float defaultSpeed = 6.0f;
    public float fuel = 100f;
    public float fuelConsumptionRate = 1f;
    public bool moveOnlyAfterLanding = false;

    [SerializeField] private Transform truckFront;
    public TextMeshProUGUI fuelDisplay;
    public string textPrefix;

    private IDriver driver;
    private Rigidbody rb;
    private float progress;
    private float lateralOffset;
    private int hits = 0;
    private bool hasLanded = true;

    void Awake()
    {
        driver = GetComponent<IDriver>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        lateralOffset = -transform.position.x;
        progress = -transform.position.z;

        if (moveOnlyAfterLanding)
        {
            hasLanded = false;
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }

    void Update()
    {
        if (fuelDisplay != null)
        {
            fuelDisplay.text = $"{textPrefix} Fuel: " + Mathf.Max(0, (int)fuel).ToString();
        }
        if (driver == null || !hasLanded) return;

        // Sync internal state with physics in case we were pushed or moved by gravity/slopes
        lateralOffset = -rb.position.x;
        progress = -rb.position.z;

        Vector3 moveVelocity = Vector3.zero;

        if (fuel > 0)
        {
            moveVelocity = driver.Move(defaultSpeed);

            float fuelConsumed = (moveVelocity.magnitude / defaultSpeed) * fuelConsumptionRate * Time.deltaTime;
            fuel -= fuelConsumed;
            if (fuel < 0) fuel = 0;
        }

        float lateralVelocity = moveVelocity.x;
        float speed = moveVelocity.z;

        lateralOffset += Time.deltaTime * lateralVelocity;
        progress += Time.deltaTime * speed;

        if (truckFront != null)
        {
            truckFront.localRotation = Quaternion.Euler(0f, 90f + lateralVelocity * steerIntensity, 0f);
        }

        rb.MovePosition(new Vector3(-lateralOffset, rb.position.y, -progress));
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "hits: " + hits);
    }


}
