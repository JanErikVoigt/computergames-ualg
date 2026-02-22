using UnityEngine;

[RequireComponent(typeof(Driver))]
public class MoveCar : MonoBehaviour
{
    public float steerIntensity = 10.0f;
    public float defaultSpeed = 6.0f;
    public float fuel = 100f;

    [SerializeField] private Transform truckFront;

    private IDriver driver;
    private float progress;
    private float lateralOffset;

    void Awake()
    {
        driver = GetComponent<IDriver>();
    }

    void Start()
    {
        progress = 0f;
        lateralOffset = 0f;
        UpdatePos();
    }

    void Update()
    {
        if (driver == null) return;

        Vector3 moveVelocity = driver.Move(defaultSpeed);

        float lateralVelocity = moveVelocity.x;
        float speed = moveVelocity.z;

        lateralOffset += Time.deltaTime * lateralVelocity;
        progress += Time.deltaTime * speed;

        if (truckFront != null)
        {
            truckFront.localRotation = Quaternion.Euler(0f, 90f + lateralVelocity * steerIntensity, 0f);
        }

        UpdatePos();
    }

    void UpdatePos()
    {
        transform.localPosition = new Vector3(-lateralOffset, 0f, -progress);
    }
}
