using TMPro;
using UnityEngine;

[RequireComponent(typeof(Driver))]
[RequireComponent(typeof(Rigidbody))]
public class MoveCar : MonoBehaviour
{
    public float steerIntensity = 10.0f;

    public float defaultSpeed = 6.0f;
    public bool moveOnlyAfterLanding = false;

    [SerializeField] private Transform truckFront;
    public TextMeshProUGUI fuelDisplay;
    public string textPrefix;

    [SerializeField] private float alignSpeed = 5f;

    private IDriver driver;
    private IFuel fuelSource;
    private Rigidbody rb;
    private Quaternion initialRotation;
    private bool hasLanded = true;

    void Awake()
    {
        driver = GetComponent<IDriver>();
        fuelSource = GetComponent<IFuel>();
        rb = GetComponent<Rigidbody>();
        initialRotation = transform.rotation;

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
            if (fuelSource != null)
            {
                fuelDisplay.text = $"{textPrefix} Fuel: " + Mathf.Max(0, (int)fuelSource.CurrentFuel).ToString();
            }
            else
            {
                fuelDisplay.text = $"{textPrefix} No Fuel System";
            }
        }
        if (driver == null || !hasLanded) return;

        Vector3 moveVelocity = Vector3.zero;

        bool hasFuel = fuelSource == null || fuelSource.HasFuel();

        if (hasFuel)
        {
            moveVelocity = driver.Move(defaultSpeed);

            if (fuelSource != null)
            {
                float normalizedMovement = moveVelocity.magnitude / defaultSpeed;
                fuelSource.Consume(normalizedMovement);
            }
        }

        float lateralVelocity = moveVelocity.x;
        float speed = moveVelocity.z;

        rb.linearVelocity = new Vector3(-lateralVelocity, rb.linearVelocity.y, -speed);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, initialRotation, alignSpeed * Time.deltaTime));

        if (truckFront != null)
        {
            truckFront.localRotation = Quaternion.Euler(0f, 90f + lateralVelocity * steerIntensity, 0f);
        }
    }
}
