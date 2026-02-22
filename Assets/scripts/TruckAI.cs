using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TruckAI : MonoBehaviour
{
    public float steerIntensity = 10.0f;
    [SerializeField] private Transform truckFront;
    public float slowSpeed = 4.0f;
    public float defaultSpeed = 6.0f;
    public float fastSpeed = 10.0f;
    public float lateralSpeed = 0.5f;
    public float fuelConsumptionRate = 0.5f;

    public Fuel fuelSystem;
    private float progress;
    private float lateralOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (fuelSystem == null)
        {
            fuelSystem = GetComponent<Fuel>();
        }
        
        if (fuelSystem == null)
        {
            Debug.LogError("Fuel script not found on " + gameObject.name + ". Please attach Fuel.cs or assign it in the inspector!");
        }

        this.progress=0f;
        this.lateralOffset=0f;
        this.updatePos();
    }

    void updatePos()
    {
        transform.localPosition = new Vector3(-this.lateralOffset, 0f, -this.progress);
    }

    // Update is called once per frame
    void Update()
    {

        float speed = 0;

        if(fuelSystem == null)
        {
            Debug.LogWarning("Fuel system not found on " + gameObject.name);
        }
        else if(fuelSystem.currentFuel <= 0)
        {
             Debug.Log("Out of fuel on " + gameObject.name);
        }

        if (fuelSystem != null && fuelSystem.currentFuel > 0)
        {
            speed = this.defaultSpeed;
            fuelSystem.currentFuel -= Time.deltaTime * fuelConsumptionRate;
        }

        this.progress += Time.deltaTime * speed;
        this.updatePos();
}

}
